using videochatspa.Server.Mappings.BaseDto;

namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class KnowledgeBaseDetailsVectorQuantityInputDto: PagedAndSortedResultRequestDto
	{
		public Guid KnowledgeBaseDetailsId { get; set; }
	}
}
