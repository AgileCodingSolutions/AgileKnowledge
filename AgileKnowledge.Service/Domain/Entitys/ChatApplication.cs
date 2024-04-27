using AgileKnowledge.Service.Domain.BaseEntity;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class ChatApplication : FullAuditedEntity
    {
        public string Name { get; set; }

        public string Prompt { get; set; } = string.Empty;

        public string ChatModel { get; set; } = "gpt-3.5-turbo";

        public double Temperature { get; set; } = 0;

        public int MaxResponseToken { get; set; } = 2000;

        public string Template { get; set; } = string.Empty;

        public string Opener { get; set; }

        public virtual ICollection<KnowledgeBase> KnowledgeBases { get; set; }

        public virtual ICollection<ChatDialog> ChatDialogs { get; set; }

        protected ChatApplication()
        {
        }
    }
}
