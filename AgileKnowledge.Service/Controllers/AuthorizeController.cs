using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Infrastructure.Helper;
using AgileKnowledge.Service.Mappings.Users;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthorizeController : ControllerBase
	{
		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;

		public AuthorizeController(KnowledgeDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<AuthorizeDto> TokenAsync(string account, string pass)
		{
			var userInfo = _dbContext.Users.FirstOrDefault(x => x.Account == account);

			if (userInfo == null)
			{
				throw new Exception("账号不存在");
			}

			if (!userInfo.CheckCipher(pass))
			{
				throw new Exception("密码错误");
			}
        
			if(userInfo.IsDisable)
			{
				throw new Exception("账号已禁用");
			}

			var dto = _mapper.Map<UserDto>(userInfo);



			return new AuthorizeDto
			{
				Token = JwtHelper.GeneratorAccessToken(dto)
			};
		}
	}
}
