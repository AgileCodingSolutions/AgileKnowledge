using videochatspa.Server.Mappings.BaseDto;

namespace AgileKnowledge.Service.Mappings.ChatApplications
{
    public class ChatAppHistory: PagedAndSortedResultRequestDto
    {
        public Guid ChatDialogId { get; set; }
    }
}
