using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
	public class CreateKnowledgeDetailsInput
	{
		public Guid FileId { get; set; }
		public string FilePath { get; set; }

        public Guid KnowledgeId { get; set; }

		public int MaxTokensPerParagraph { get; set; }

		public int MaxTokensPerLine { get; set; }

		public int OverlappingTokens { get; set; }

		public TrainingPatternType TrainingPattern { get; set; } = TrainingPatternType.QA;
    
		public string? QAPromptTemplate { get; set; }
	}
}
