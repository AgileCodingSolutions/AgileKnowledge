using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgileKnowledge.Service.Mappings.Users;
using AgileKnowledge.Service.Options;
using Microsoft.IdentityModel.Tokens;

namespace AgileKnowledge.Service.Infrastructure.Helper
{
	public class JwtHelper
	{
		public static string GeneratorAccessToken(ClaimsIdentity claimsIdentity)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(JwtOptions.Secret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = claimsIdentity,
				Expires = DateTime.UtcNow.AddHours(JwtOptions.EffectiveHours),
				SigningCredentials =
					new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public static string GeneratorAccessToken(UserDto user)
		{
			var claimsIdentity = GetClaimsIdentity(user);

			return GeneratorAccessToken(claimsIdentity);
		}

		public static ClaimsIdentity GetClaimsIdentity(UserDto user)
		{
			return new ClaimsIdentity(new Claim[]
			{
				new(ClaimTypes.Name, user.Account),
				new(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new(ClaimTypes.Role, user.RoleName.ToString()),
				new("IsDisable", user.IsDisable.ToString()),
			});
		}

	}
}
