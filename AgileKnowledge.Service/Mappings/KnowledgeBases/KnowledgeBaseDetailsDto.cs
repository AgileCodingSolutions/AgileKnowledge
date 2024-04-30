using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class KnowledgeBaseDetailsDto
	{
		public KnowledgeBaseQuantizationState State { get; set; }
		public int MaxTokensPerParagraph { get; set; }
		public int MaxTokensPerLine { get; set; }
		public int OverlappingTokens { get; set; }
		public TrainingPatternType TrainingPattern { get; set; } 
		public string QAPromptTemplate { get; set; } 
		public int DataCount { get; set; }


		public Guid LastModifierId { get; set; }
		public DateTime? LastModificationTime { get; set; }

	}
}
