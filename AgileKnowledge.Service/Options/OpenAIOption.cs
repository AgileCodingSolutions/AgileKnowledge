namespace AgileKnowledge.Service.Options
{
	public class OpenAIOption
	{
		public const string Name = "OpenAI";

		public static string ChatEndpoint { get; set; }

		public static string EmbeddingEndpoint { get; set; }

		public static string ChatToken { get; set; }

		public static string EmbeddingToken { get; set; }
    
		public static string EmbeddingModel { get; set; } = "text-embedding-3-small";
	}
}
