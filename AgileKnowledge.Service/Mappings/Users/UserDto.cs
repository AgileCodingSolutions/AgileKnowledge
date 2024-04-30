using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsDisable { get; set; }
        public RoleType Role { get; set; }

        public string RoleName
        {
            get
            {
                switch (Role)
                {
                    case RoleType.Admin:
                        return "Admin";
                    case RoleType.User:
                        return "User";
                }

                return "User";
            }
        }
    }
}
