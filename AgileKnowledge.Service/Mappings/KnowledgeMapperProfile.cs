using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Domain.Entitys;
using AgileKnowledge.Service.Mappings.ChatApplications;
using AgileKnowledge.Service.Mappings.KnowledgeBases;
using AgileKnowledge.Service.Mappings.Users;
using AutoMapper;

namespace AgileKnowledge.Service.Mappings
{
	public class KnowledgeMapperProfile:Profile
	{
		public KnowledgeMapperProfile()
		{
			CreateMap<User, UserDto>();

			CreateMap<KnowledgeBase, KnowledgeBasesDto>();

			CreateMap<ChatApplication, ChatApplicationDto>();

            CreateMap<KnowledgeBaseDetails, KnowledgeBaseDetailsDto>();

			CreateMap<ChatApplication, PostShareDto>();

			CreateMap<ChatDialog, ChatDialogDto>();

			CreateMap<ChatDialogHistory, CreateChatDialogHistoryInputDto>();
        }
    }
}
