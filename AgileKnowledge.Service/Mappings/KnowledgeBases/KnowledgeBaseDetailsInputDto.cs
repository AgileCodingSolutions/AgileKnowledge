using AgileKnowledge.Service.Domain.Enum;
using videochatspa.Server.Mappings.BaseDto;

namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class KnowledgeBaseDetailsInputDto: PagedAndSortedResultRequestDto
	{
		public Guid KnowledgeBaseId { get; set; }

		public KnowledgeBaseQuantizationState?  State { get; set; }
	}
}
