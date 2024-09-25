using AgileKnowledge.Service.Domain.BaseEntity;
using videochatspa.Server.Mappings.BaseDto;

namespace AgileKnowledge.Service.Mappings.ChatApplications
{
    public class PostShareList : PagedAndSortedResultRequestDto
    {
      public Guid ChatApplicationId {  get; set; }
    }
}
