using AgileKnowledge.Service.Options;

using Microsoft.KernelMemory.AI.OpenAI;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.DataFormats.Text;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Extensions;
using Microsoft.KernelMemory.Pipeline;

namespace AgileKnowledge.Service.Service
{
	public class QAHandler : IPipelineStepHandler
	{
		private readonly TextPartitioningOptions _options;
		private readonly IPipelineOrchestrator _orchestrator;
		private readonly KnowledgeMemoryService _knowledgeMemoryService;
		private readonly ILogger<QAHandler> _log;
		private readonly TextChunker.TokenCounter _tokenCounter;


		public string StepName { get; }

		public QAHandler(
			string stepName,
			IPipelineOrchestrator orchestrator, KnowledgeMemoryService knowledgeMemoryService,
			TextPartitioningOptions? options = null,
			ILogger<QAHandler>? log = null
		)
		{
			this.StepName = stepName;
			this._orchestrator = orchestrator;
			_knowledgeMemoryService = knowledgeMemoryService;
			this._options = options ?? new TextPartitioningOptions();
			this._options.Validate();

			this._log = log ?? DefaultLogger<QAHandler>.Instance;
			this._tokenCounter = DefaultGPTTokenizer.StaticCountTokens;
		}



		public async Task<(bool success, DataPipeline updatedPipeline)> InvokeAsync(DataPipeline pipeline, CancellationToken cancellationToken = new CancellationToken())
		{
			this._log.LogDebug("Partitioning text, pipeline '{0}/{1}'", pipeline.Index, pipeline.DocumentId);

			if (pipeline.Files.Count == 0)
			{
				this._log.LogWarning("Pipeline '{0}/{1}': there are no files to process, moving to next pipeline step.",
					pipeline.Index, pipeline.DocumentId);
				return (true, pipeline);
			}


			foreach (DataPipeline.FileDetails uploadedFile in pipeline.Files)
			{

				Dictionary<string, DataPipeline.GeneratedFileDetails> newFiles = new();

				  foreach (KeyValuePair<string, DataPipeline.GeneratedFileDetails> generatedFile in uploadedFile
                         .GeneratedFiles)
            {
                var file = generatedFile.Value;
                if (file.AlreadyProcessedBy(this))
                {
                    this._log.LogTrace("File {0} already processed by this handler", file.Name);
                    continue;
                }

                // Partition only the original text
                if (file.ArtifactType != DataPipeline.ArtifactTypes.ExtractedText)
                {
                    this._log.LogTrace("Skipping file {0} (not original text)", file.Name);
                    continue;
                }

                // Use a different partitioning strategy depending on the file type
                List<string> partitions = new();
                List<string> sentences = new();
                BinaryData partitionContent = await this._orchestrator
                    .ReadFileAsync(pipeline, file.Name, cancellationToken).ConfigureAwait(false);

                // Skip empty partitions. Also: partitionContent.ToString() throws an exception if there are no bytes.
                if (partitionContent.ToArray().Length == 0)
                {
                    continue;
                }

                switch (file.MimeType)
                {
                    case MimeTypes.PlainText:
                    case MimeTypes.MarkDown:
                    {
                        _log.LogDebug("Partitioning text file {0}", file.Name);
                        string content = partitionContent.ToString();

                        if (QuantizeBackgroundService.CacheKnowledgeBaseDetails.TryGetValue(StepName, out var wikiDetail))
                        {
                            await foreach (var item in OpenAIService
                                               .QaAsync(wikiDetail.Item1.QAPromptTemplate, content,
                                                   wikiDetail.Item2.Model, OpenAIOption.ChatToken,
                                                   OpenAIOption.ChatEndpoint, _knowledgeMemoryService)
                                               .WithCancellation(cancellationToken))
                            {
                                partitions.Add(item);
                                sentences.Add(item);
                            }
                        }


                        break;
                    }
                    default:
                        this._log.LogWarning("File {0} cannot be partitioned, type '{1}' not supported", file.Name,
                            file.MimeType);
                        // Don't partition other files
                        continue;
                }

                if (partitions.Count == 0)
                {
                    continue;
                }

                this._log.LogDebug("Saving {0} file partitions", partitions.Count);
                for (int partitionNumber = 0; partitionNumber < partitions.Count; partitionNumber++)
                {
                    // TODO: turn partitions in objects with more details, e.g. page number
                    string text = partitions[partitionNumber];
                    int sectionNumber = 0; // TODO: use this to store the page number (if any)
                    BinaryData textData = new(text);

                    int tokenCount = this._tokenCounter(text);
                    this._log.LogDebug("Partition size: {0} tokens", tokenCount);

                    var destFile = uploadedFile.GetPartitionFileName(partitionNumber);
                    await this._orchestrator.WriteFileAsync(pipeline, destFile, textData, cancellationToken)
                        .ConfigureAwait(false);

                    var destFileDetails = new DataPipeline.GeneratedFileDetails
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ParentId = uploadedFile.Id,
                        Name = destFile,
                        Size = text.Length,
                        MimeType = MimeTypes.PlainText,
                        ArtifactType = DataPipeline.ArtifactTypes.TextPartition,
                        PartitionNumber = partitionNumber,
                        SectionNumber = sectionNumber,
                        Tags = pipeline.Tags,
                        ContentSHA256 = textData.CalculateSHA256(),
                    };
                    newFiles.Add(destFile, destFileDetails);
                    destFileDetails.MarkProcessedBy(this);
                }

                file.MarkProcessedBy(this);
            }

            // Add new files to pipeline status
            foreach (var file in newFiles)
            {
                uploadedFile.GeneratedFiles.Add(file.Key, file.Value);
            }

			}

			return (true, pipeline);
		}


	}
}
