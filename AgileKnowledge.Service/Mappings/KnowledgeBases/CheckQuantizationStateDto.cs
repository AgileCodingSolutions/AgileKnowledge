using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class CheckQuantizationStateDto
	{
		public KnowledgeBaseQuantizationState State { get; set; }

		public Guid KnowledgeBaseDetailsId { get; set; }

		public string FileName { get; set; }

	}
}
