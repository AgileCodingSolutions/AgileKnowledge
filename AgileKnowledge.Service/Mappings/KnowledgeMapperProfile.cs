using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Mappings.Users;
using AutoMapper;

namespace AgileKnowledge.Service.Mappings
{
	public class KnowledgeMapperProfile:Profile
	{
		public KnowledgeMapperProfile()
		{
			CreateMap<User, UserDto>();
		}
	}
}
