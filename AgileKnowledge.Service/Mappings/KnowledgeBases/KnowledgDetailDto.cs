using AgileKnowledge.Service.Domain.Enum;
using System;
namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
    public sealed class KnowledgDetailDto
    {
        public Guid Id { get; set; }

        
        public Guid KnowledgeBaseld { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public int DataCount { get; set; }

        
        public string Type { get; set; }

        public long Creator { get; }

        public DateTime CreationTime { get; set; }
        public WikiQuantizationState State { get; set; }

        public string StateName
        {
            get
            {
                switch (State)
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

        public long Modifier { get; set; }

        public DateTime ModificationTime { get; set; }
    }


}
