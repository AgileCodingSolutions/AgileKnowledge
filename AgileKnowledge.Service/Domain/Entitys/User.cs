using System.Text.RegularExpressions;
using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Enum;
using AgileKnowledge.Service.Helper;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class User : FullAuditedEntity
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsDisable { get; set; }
        public RoleType Role { get; set; }

        protected User()
        {
        }

        public User(string account, string name, string password, string avatar, string email, string phone,
            bool isDisable, RoleType role = RoleType.User)
        {
            Id = Guid.NewGuid();
            SetPassword(password);
            SetEmail(email);
            Account = account;
            Name = name;
            Avatar = avatar;
            Phone = phone;
            IsDisable = isDisable;
            SetUserRole();
        }

        public void Disable()
        {
            IsDisable = true;
        }

        public void Enable()
        {
            IsDisable = false;
        }

        public void SetAdminRole()
        {
            Role = RoleType.Admin;
        }

        public void SetUserRole()
        {
            Role = RoleType.User;
        }

        public void SetPassword(string password)
        {
            Salt = Guid.NewGuid().ToString("N");
            Password = Md5Helper.HashPassword(password, Salt);
        }

        public void SetEmail(string email)
        {
            var regex = new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
            if (!regex.IsMatch(email))
            {
                throw new ArgumentException("Email format error");
            }
            Email = email;
        }

        public bool CheckCipher(string password)
        {
            if (Password == Md5Helper.HashPassword(password, Salt))
            {
                return true;
            }
            return false;
        }
    }
}
