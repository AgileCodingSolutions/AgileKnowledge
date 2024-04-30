using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.Users
{
	public class ChangeRoleInputDto
	{
		public Guid Id { get; set; }
		public RoleType Role { get; set; }
	}
}
