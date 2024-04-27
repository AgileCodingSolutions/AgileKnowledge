using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class ChatDialogHistory : FullAuditedEntity
    {
        public string Content { get; set; }
        public int TokenConsumption { get; set; }
        public ChatDialogHistoryType Type { get; set; }
        public ChatDialog ChatDialog { get; set; }
        protected ChatDialogHistory()
        {
        }
    }
}
