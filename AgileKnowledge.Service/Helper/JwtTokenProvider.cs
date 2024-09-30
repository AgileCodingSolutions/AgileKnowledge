using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgileKnowledge.Service.Domain.Enum;
using Microsoft.IdentityModel.Tokens;

namespace AgileKnowledge.Service.Helper
{
	public class JwtTokenProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;

		public JwtTokenProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}

		public Guid GetUserId()
		{
			if (_httpContextAccessor == null)
			{
				return Guid.Empty;
			}
			if (_httpContextAccessor.HttpContext == null)
			{
				return Guid.Empty;
			}


			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim?.Value == null)
			{
				return Guid.Empty;
			}

			return Guid.Parse(userIdClaim?.Value);
		}

		public string GenerateToken(Guid userId, RoleType role)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
				new Claim(ClaimTypes.Role, role.ToString()),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				_configuration["Authentication:Issuer"],
				_configuration["Authentication:Audience"],
				claims,
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddDays(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

        public string? ValidateResetPasswordToken(string token)
        {
            //TODO: EMAIL SEND REDIS TOKEN VERIFY
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Authentication:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Authentication:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                // valid token expires
                if (validatedToken.ValidTo >= DateTime.UtcNow)
                {
                    var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        return userIdClaim.Value;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
