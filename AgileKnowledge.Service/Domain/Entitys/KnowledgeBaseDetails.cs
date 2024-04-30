using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class KnowledgeBaseDetails : FullAuditedEntity
    {

        public KnowledgeBaseQuantizationState State { get; set; }

        public int MaxTokensPerParagraph { get; set; }

        public int MaxTokensPerLine { get; set; }

        public int OverlappingTokens { get; set; }

        public TrainingPatternType TrainingPattern { get; set; } = TrainingPatternType.QA;
        public string? QAPromptTemplate { get; set; } =
            """"
			我会给你一段文本，学习它们，并整理学习成果，要求为：
			1. 提出最多 20 个问题。
			2. 给出每个问题的答案。
			3. 答案要详细完整，答案可以包含普通文字、链接、代码、表格、公示、媒体链接等 markdown 元素。
			4. 按格式返回多个问题和答案:

			Q1: 问题。
			A1: 答案。
			Q2:
			A2:
			……

			我的文本："""{{$input}}"""
			"""";

        public FileStorage File { get; set; }
        public int DataCount { get; set; }

        public KnowledgeBase KnowledgeBase { get; set; }

        protected KnowledgeBaseDetails()
        {
        }

        public KnowledgeBaseDetails(FileStorage file,KnowledgeBase knowledgeBase)
        {
            DataCount = 0;
            State = KnowledgeBaseQuantizationState.None;


            this.File = file;
            this.KnowledgeBase = knowledgeBase;
        }
    }
}
