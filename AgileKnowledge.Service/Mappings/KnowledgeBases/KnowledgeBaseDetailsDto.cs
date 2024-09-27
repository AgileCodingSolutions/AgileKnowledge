using AgileKnowledge.Service.Domain.Enities;
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
        public FileStorage File { get; set; }

        public KnowledgeBase KnowledgeBase { get; set; }
        public Guid LastModifierId { get; set; }
		public DateTime? LastModificationTime { get; set; }

	}

    public class KnowledgeBaseDetailsViewDto
    {
        public KnowledgeBaseQuantizationState State { get; set; }
        public int MaxTokensPerParagraph { get; set; }
        public int MaxTokensPerLine { get; set; }
        public int OverlappingTokens { get; set; }
        public TrainingPatternType TrainingPattern { get; set; }
        public string QAPromptTemplate { get; set; }
        public int DataCount { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Guid Id { get; set; }
        public long Creator { get; }

        public DateTime CreationTime { get; set; }
        public WikiQuantizationState State1 { get; set; }

        public string StateName
        {
            get
            {
                switch (State1)
                {
                    case WikiQuantizationState.None:
                        return "处理中";
                    case WikiQuantizationState.Accomplish:
                        return "已完成";
                    case WikiQuantizationState.Fail:
                        return "失败";
                }

                return "错误状态";
            }
        }

        public KnowledgeBase KnowledgeBase { get; set; }
        public Guid LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

    }
}
