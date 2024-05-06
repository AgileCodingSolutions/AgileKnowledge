namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class SearchVectorQuantityDto
	{
		public double ElapsedTime { get; set; }

		public List<SearchVectorQuantity> Result { get; set; } = new();
	}
}
