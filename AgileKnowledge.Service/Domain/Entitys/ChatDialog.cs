using AgileKnowledge.Service.Domain.BaseEntity;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class ChatDialog : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ChatApplication ChatApplication { get; set; }
        public virtual ICollection<ChatDialogHistory> ChatDialogHistorys { get; set; }
        protected ChatDialog()
        {
        }
    }
}
