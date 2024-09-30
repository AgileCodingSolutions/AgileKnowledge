using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Domain.Enum;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;

namespace AgileKnowledge.Service.Service
{
	public class QuantizeBackgroundService: BackgroundService
	{

		private readonly ILogger<QuantizeBackgroundService> _logger;
		private readonly IServiceProvider _serviceProvider;


		public static ConcurrentDictionary<string, (KnowledgeBaseDetails, KnowledgeBase)> CacheKnowledgeBaseDetails { get; } = new();
		private static int _maxTask = 1;


		private static int _currentTask;
		private static readonly Channel<KnowledgeBaseDetails> KnowledgeBaseDetails = Channel.CreateBounded<KnowledgeBaseDetails>(
			new BoundedChannelOptions(1000)
			{
				SingleReader = true,
				SingleWriter = false
			});


		public QuantizeBackgroundService(IServiceProvider serviceProvider, ILogger<QuantizeBackgroundService> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)

		{
			var QUANTIZE_MAX_TASK = "1";//Environment.GetEnvironmentVariable("QUANTIZE_MAX_TASK");
			if (!string.IsNullOrEmpty(QUANTIZE_MAX_TASK))
			{
				int.TryParse(QUANTIZE_MAX_TASK, out _maxTask);
			}
			if (_maxTask < 0)
			{
				_maxTask = 1;
			}


			await LoadingKnowledgeBaseDetailsAsync();


			var tasks = new List<Task>();
			for (var i = 0; i < _maxTask; i++)
			{
				tasks.Add(Task.Factory.StartNew(KnowledgeBaseDetailsHandlerAsync, stoppingToken));
			}

			await Task.WhenAll(tasks);
		}



		private async Task KnowledgeBaseDetailsHandlerAsync()
		{
			using var asyncServiceScope = _serviceProvider.CreateScope();
			while (await KnowledgeBaseDetails.Reader.WaitToReadAsync())
			{
				Interlocked.Increment(ref _currentTask);
				_logger.LogInformation($"当前任务数量：{_currentTask}");
				var knowledgeBaseDetails = await KnowledgeBaseDetails.Reader.ReadAsync();
				await HandlerAsync(knowledgeBaseDetails, asyncServiceScope.ServiceProvider);
				Interlocked.Decrement(ref _currentTask);
			}
		}

		private async ValueTask HandlerAsync(KnowledgeBaseDetails knowledgeBaseDetails, IServiceProvider service)
		{
			var dbContext = service.GetRequiredService<KnowledgeDbContext>();
			var knowledgeMemoryService = service.GetRequiredService<KnowledgeMemoryService>();


			CacheKnowledgeBaseDetails.TryAdd(knowledgeBaseDetails.Id.ToString(), new ValueTuple<KnowledgeBaseDetails, KnowledgeBase>(knowledgeBaseDetails, knowledgeBaseDetails.KnowledgeBase));

			knowledgeBaseDetails.MaxTokensPerLine = 300;
			knowledgeBaseDetails.MaxTokensPerParagraph = 1000;
			knowledgeBaseDetails.OverlappingTokens = 100;


			var serverless = knowledgeMemoryService.CreateMemoryServerless(new SearchClientConfig(),
				knowledgeBaseDetails.MaxTokensPerLine, knowledgeBaseDetails.MaxTokensPerParagraph, knowledgeBaseDetails.OverlappingTokens, knowledgeBaseDetails.KnowledgeBase?.Model,
				knowledgeBaseDetails.KnowledgeBase?.EmbeddingModel);


			try
			{
				_logger.LogInformation($"开始量化：{knowledgeBaseDetails.File.FullName} {knowledgeBaseDetails.File.Path} {knowledgeBaseDetails.File.Id}");
				var step = new List<string>();
				if (knowledgeBaseDetails.TrainingPattern == TrainingPatternType.QA)
				{
					var stepName = knowledgeBaseDetails.Id.ToString();
					serverless.Orchestrator.AddHandler<TextExtractionHandler>("extract_text");
				    //serverless.Orchestrator.AddHandler<QAHandler>(stepName);
					serverless.Orchestrator.AddHandler<GenerateEmbeddingsHandler>("generate_embeddings");
					serverless.Orchestrator.AddHandler<SaveRecordsHandler>("save_memory_records");
					step.Add("extract_text");
					step.Add(stepName);
					step.Add("generate_embeddings");
					step.Add("save_memory_records");
				}

				string result = string.Empty;
				if (knowledgeBaseDetails.File.Type == "file")
				{
					var fileInfoQuery = knowledgeBaseDetails.File;
                    var currentDirectory = Directory.GetCurrentDirectory();
					var path = Path.Combine(currentDirectory, "wwwroot", fileInfoQuery.Path);


                    result = await serverless.ImportDocumentAsync(path,
						knowledgeBaseDetails.Id.ToString(),
						tags: new TagCollection()
						{
							{
								"wikiId", knowledgeBaseDetails.KnowledgeBase.Id.ToString()
							},
							{
								"fileId", knowledgeBaseDetails.File.Id.ToString()
							},
							{
								"wikiDetailId", knowledgeBaseDetails.Id.ToString()
							}
						}, "wiki");
				}
				else if (knowledgeBaseDetails.File.Type == "web")
				{
					result = await serverless.ImportWebPageAsync(knowledgeBaseDetails.File.Path,
						knowledgeBaseDetails.Id.ToString(),
						tags: new TagCollection()
						{
							{
								"wikiId", knowledgeBaseDetails.KnowledgeBase.Id.ToString()
							},
							{
								"wikiDetailId", knowledgeBaseDetails.Id.ToString()
							}
						}, "wiki", steps: step.ToArray());
				}
				else if (knowledgeBaseDetails.File.Type == "data")
				{
					result = await serverless.ImportDocumentAsync(knowledgeBaseDetails.File.Path,
						knowledgeBaseDetails.Id.ToString(),
						tags: new TagCollection()
						{
							{
								"wikiId", knowledgeBaseDetails.KnowledgeBase.Id.ToString()
							},
							{
								"wikiDetailId", knowledgeBaseDetails.Id.ToString()
							}
						}, "wiki", steps: step.ToArray());
				}

				knowledgeBaseDetails.State = KnowledgeBaseQuantizationState.Success;
				//dbContext.Update(knowledgeBaseDetails);
				//await dbContext.SaveChangesAsync();
				_logger.LogInformation($"量化成功：{knowledgeBaseDetails.File.FullName} {knowledgeBaseDetails.File.Path} {knowledgeBaseDetails.File.Id} {result}");

			}
			catch (Exception e)
			{
				_logger.LogError(e, $"量化失败{knowledgeBaseDetails.File.FullName} {knowledgeBaseDetails.File.Path} {knowledgeBaseDetails.File.Id}");

				if (knowledgeBaseDetails.State != KnowledgeBaseQuantizationState.Failed)
				{
					knowledgeBaseDetails.State = KnowledgeBaseQuantizationState.Failed;
					dbContext.Update(knowledgeBaseDetails);
					await dbContext.SaveChangesAsync();
				}
			}
			finally
			{
				CacheKnowledgeBaseDetails.Remove(knowledgeBaseDetails.Id.ToString(), out _);
			}

		}

		private async Task LoadingKnowledgeBaseDetailsAsync()
		{
			using var asyncServiceScope = _serviceProvider.CreateScope();
			var dbContext = asyncServiceScope.ServiceProvider.GetRequiredService<KnowledgeDbContext>();
			foreach (var knowledgeBaseDetail in await dbContext.KnowledgeBaseDetails.Include(x=>x.KnowledgeBase).Include(x=>x.File).Where(x=>x.State == KnowledgeBaseQuantizationState.Failed || x.State == KnowledgeBaseQuantizationState.None).ToListAsync())
			{
				await AddKnowledgeBaseDetailAsync(knowledgeBaseDetail);
			}
		}

		public static async Task AddKnowledgeBaseDetailAsync(KnowledgeBaseDetails knowledgeBaseDetail)
		{
			await KnowledgeBaseDetails.Writer.WriteAsync(knowledgeBaseDetail);
		}


	}
}
