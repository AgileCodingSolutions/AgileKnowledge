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
					.WithOpenAITextGeneration(new OpenAIConfig()
					{
						APIKey = OpenAIOption.ChatToken,
						TextModel = chatModel,
					})
					.WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
					{
						APIKey = string.IsNullOrEmpty(OpenAIOption.EmbeddingToken)
							? OpenAIOption.ChatToken
							: OpenAIOption.EmbeddingToken,
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
                    //.WithOpenAITextGeneration(new OpenAIConfig()
                    //{
                    //	APIKey = OpenAIOption.ChatToken,
                    //	TextModel = chatModel
                    //}, null, new HttpClient(HttpClientHandler))
                    //.WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
                    //{
                    //	APIKey = string.IsNullOrEmpty(OpenAIOption.EmbeddingToken)
                    //		? OpenAIOption.ChatToken
                    //		: OpenAIOption.EmbeddingToken,
                    //	EmbeddingModel = embeddingModel,
                    //}, null, false, new HttpClient(HttpClientHandler))
                    .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                    {
                        Deployment = "text-embedding-3-large-1",
                        Endpoint = "https://shinetech-openai.openai.azure.com/",
                        Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                        APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                        APIKey = "7eca0250381048d0938e07c3ce0ec0d9"
                    })
					.WithAzureOpenAITextGeneration(new AzureOpenAIConfig
					{
						Deployment = "gpt35-1",
						Endpoint = "https://shinetech-openai.openai.azure.com/",
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIKey = "7eca0250381048d0938e07c3ce0ec0d9",
						APIType = AzureOpenAIConfig.APITypes.ChatCompletion
					})
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
					.WithOpenAITextGeneration(new OpenAIConfig()
					{
						APIKey = OpenAIOption.ChatToken,
						TextModel = model ?? OpenAIOption.ChatToken
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
			else
			{
				return new KernelMemoryBuilder()
					.WithPostgresMemoryDb(new PostgresConfig()
					{
						ConnectionString = ConnectionStringsOptions.DefaultConnection,
						TableNamePrefix = ConnectionStringsOptions.TableNamePrefix
					})
					// 配置文档解析向量模型
					.WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
					{
						Deployment = "text-embedding-3-large-1",
						Endpoint = "https://shinetech-openai.openai.azure.com/",
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
						APIKey = "7eca0250381048d0938e07c3ce0ec0d9"
					})
					// 配置文本生成模型
					.WithAzureOpenAITextGeneration(new AzureOpenAIConfig
					{
						Deployment = "gpt35-1",
						Endpoint = "https://shinetech-openai.openai.azure.com/",
						Auth = AzureOpenAIConfig.AuthTypes.APIKey,
						APIKey = "7eca0250381048d0938e07c3ce0ec0d9",
						APIType = AzureOpenAIConfig.APITypes.ChatCompletion
					})
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
