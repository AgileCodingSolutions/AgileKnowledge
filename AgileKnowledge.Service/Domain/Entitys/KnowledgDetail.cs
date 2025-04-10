﻿using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Enum;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AgileKnowledge.Service.Domain.Entitys
{
    public class KnowledgDetail : FullAuditedEntity
    {
        public string WikiId { get; set; }


        public string FileName { get; set; }

        public string Path { get; set; }

        public int DataCount { get; set; }


        public string Type { get; set; }

        public long Creator { get; }

        public DateTime CreationTime { get; set; }

        public WikiQuantizationState State { get; set; }

        public long Modifier { get; set; }

        public DateTime ModificationTime { get; set; }

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

    }
}
