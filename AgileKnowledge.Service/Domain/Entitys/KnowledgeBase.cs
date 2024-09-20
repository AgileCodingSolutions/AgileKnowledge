using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Entitys;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class KnowledgeBase : FullAuditedEntity
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string EmbeddingModel { get; set; }
        public virtual ICollection<KnowledgeBaseDetails> KnowledgeBaseDetails { get; set; }
        
        protected KnowledgeBase()
        {
        }

        public KnowledgeBase(string icon, string name, string model, string embeddingModel)
        {
            Icon = icon;
            Name = name;
            Model = model;
            EmbeddingModel = embeddingModel;
        }





    }
}
