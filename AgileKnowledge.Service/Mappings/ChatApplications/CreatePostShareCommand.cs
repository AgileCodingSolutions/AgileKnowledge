using AgileKnowledge.Service.Domain.BaseEntity;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AgileKnowledge.Service.Mappings.ChatApplications
{
    public class CreatePostShareCommand: FullAuditedEntity
    {
        public string Name { get; set; }

        public string ChatApplicationId { get; set; }

        public DateTime Expires { get; set; } = DateTime.Now.AddDays(7);

        public long AvailableToken { get; set; } = -1;

        public int AvailableQuantity { get; set; } = -1;
        protected CreatePostShareCommand()
        {
        }
        public CreatePostShareCommand(string name,string chatApplicationId, DateTime expires, long availableToken, int availableQuantity)
        {
            Name = name;
            ChatApplicationId = chatApplicationId;
            Expires = expires;
            AvailableToken = availableToken;
            AvailableQuantity = availableQuantity;
        }
    }
    
}
