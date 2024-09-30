using AgileKnowledge.Service.Options;

using Microsoft.IdentityModel.Tokens;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using Microsoft.KernelMemory.Postgres;
using Microsoft.SemanticKernel;

namespace AgileKnowledge.Service.Service
{
	public class KnowledgeMemoryService
	{
		private static readonly OpenAiHttpClientHandler HttpClientHandler = new();


		public MemoryServerless CreateMemoryServerless(SearchClientConfig searchClientConfig,
			int maxTokensPerLine,
			int maxTokensPerParagraph,
			int overlappingTokens,
			string? chatModel, string? embeddingModel)
		{
			if (string.IsNullOrEmpty(ConnectionStringsOptions.DefaultConnection))
			{
				var memory = new KernelMemoryBuilder()
					.WithSimpleVectorDb(new SimpleVectorDbConfig()
					{
						StorageType = FileSystemTypes.Disk,
						Directory = "./data"
					})
					.WithSearchClientConfig(searchClientConfig)
					.WithCustomTextPartitioningOptions(new TextPartitioningOptions()
					{
						MaxTokensPerLine = maxTokensPerLine,
						MaxTokensPerParagraph = maxTokensPerParagraph,
						OverlappingTokens = overlappingTokens,
					})
					.WithAzureOpenAITextGeneration(new AzureOpenAIConfig
					{
						Deployment = OpenAIOption.ChatModel,
						Endpoint = OpenAIOption.ChatEndpoint,
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIKey = OpenAIOption.ChatToken,
						APIType = AzureOpenAIConfig.APITypes.ChatCompletion
					})
					.WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
					{
						Deployment = OpenAIOption.EmbeddingModel,
						Endpoint = OpenAIOption.EmbeddingEndpoint,
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
						APIKey = OpenAIOption.EmbeddingToken
					})
					.AddSingleton(new KnowledgeMemoryService())
					.Build<MemoryServerless>();

				return memory;
			}
			else
			{

				var memory = new KernelMemoryBuilder()
					.WithPostgresMemoryDb(new PostgresConfig()
					{
						ConnectionString = ConnectionStringsOptions.DefaultConnection,
						TableNamePrefix = ConnectionStringsOptions.TableNamePrefix
					})
					.WithSimpleFileStorage(new SimpleFileStorageConfig
					{
						StorageType = FileSystemTypes.Volatile,
						Directory = "_files"
					})
					.WithSearchClientConfig(searchClientConfig)
					.WithCustomTextPartitioningOptions(new TextPartitioningOptions()
					{
						MaxTokensPerLine = maxTokensPerLine,
						MaxTokensPerParagraph = maxTokensPerParagraph,
						OverlappingTokens = overlappingTokens
					})
					.WithOpenAITextGeneration(new OpenAIConfig()
					{
						APIKey = OpenAIOption.ChatToken,
						TextModel = chatModel
					}, null, new HttpClient(HttpClientHandler))
					.WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
					{
						APIKey = string.IsNullOrEmpty(OpenAIOption.EmbeddingToken)
							? OpenAIOption.ChatToken
							: OpenAIOption.EmbeddingToken,
						EmbeddingModel = embeddingModel,
					}, null, false, new HttpClient(HttpClientHandler))
					.Build<MemoryServerless>();

				return memory;
			}
		}


		public MemoryServerless CreateMemoryServerless(string? model = null)
		{
			if (ConnectionStringsOptions.DefaultConnection.IsNullOrEmpty())
			{
				return new KernelMemoryBuilder()
					.WithSimpleVectorDb(new SimpleVectorDbConfig
					{
						StorageType = FileSystemTypes.Disk,
						Directory = "./data"
					})
					.WithAzureOpenAITextGeneration(new AzureOpenAIConfig
					{
						Deployment = OpenAIOption.ChatModel,
						Endpoint = OpenAIOption.ChatEndpoint,
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIKey = OpenAIOption.ChatToken,
						APIType = AzureOpenAIConfig.APITypes.ChatCompletion
					})
					.WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
					{
						Deployment = OpenAIOption.EmbeddingModel,
						Endpoint = OpenAIOption.EmbeddingEndpoint,
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
						APIKey = OpenAIOption.EmbeddingToken
					})
					.Build<MemoryServerless>();
			}
			else
			{
				return new KernelMemoryBuilder()
					.WithPostgresMemoryDb(new PostgresConfig()
					{
						ConnectionString = ConnectionStringsOptions.DefaultConnection,
						TableNamePrefix = ConnectionStringsOptions.TableNamePrefix
					})
					.WithOpenAITextGeneration(new OpenAIConfig()
					{
						APIKey = OpenAIOption.ChatToken,
						TextModel = model ?? string.Empty
					}, null, new HttpClient(HttpClientHandler))
					.WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
					{
						// 如果 EmbeddingToken 为空，则使用 ChatToken
						APIKey = string.IsNullOrEmpty(OpenAIOption.EmbeddingToken)
							? OpenAIOption.ChatToken
							: OpenAIOption.EmbeddingToken,
						EmbeddingModel = OpenAIOption.EmbeddingModel,
					}, null, false, new HttpClient(HttpClientHandler))
					.Build<MemoryServerless>();
			}
		}



		public Kernel CreateFunctionKernel(string apiKey, string modelId, string uri)
		{
			var kernel = Kernel.CreateBuilder()
				.AddOpenAIChatCompletion(
					modelId: modelId,
					apiKey: apiKey,
					httpClient: new HttpClient(new OpenAiHttpClientHandler(uri)))
				.Build();

			return kernel;
		}

	}
}
