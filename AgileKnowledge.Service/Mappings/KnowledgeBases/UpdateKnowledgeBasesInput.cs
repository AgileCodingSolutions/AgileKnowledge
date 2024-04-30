namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class UpdateKnowledgeBasesInput
	{
		public Guid Id { get; set; }
		public string Icon { get; set; }
		public string Name { get; set; }
		public string Model { get; set; } = "gpt-3.5-turbo";
		public string EmbeddingModel { get; set; } = "text-embedding-3-small";
	}
}
