using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Mappings;
using AgileKnowledge.Service.Mappings.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;
using videochatspa.Server.Mappings.BaseDto;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Helper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly JwtTokenProvider _jwtTokenProvider;

		public UserController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_jwtTokenProvider = jwtTokenProvider;
		}

		[HttpGet]
		public async Task<PagedResultDto<UserDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
		{
			var query = _dbContext.Users.WhereIf(!string.IsNullOrEmpty(input.Filter),
				x => x.Account.Contains(input.Filter));

			var total = query.Count();

			var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

			return new PagedResultDto<UserDto>(total, _mapper.Map<List<UserDto>>(list));
		}

		[HttpPost]
		public async Task CreateAsync([FromBody] CreateUserInput input)
		{

			if (input.Account.Length < 6 || input.Account.Length > 20)
				throw new Exception("Account length must be between 6-20");
        
			if (input.Password.Length < 6 || input.Password.Length > 20)
				throw new Exception("Password length must be between 6-20");
			
			if (!Regex.IsMatch(input.Email, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$"))
				throw new Exception("Email format error");
        
			// TODO: 验证账号是否存在
			if(await _dbContext.Users.AnyAsync(x => x.Account == input.Account))
				throw new Exception("Account number already exists");
        
			var user = new User(input.Account, input.Name, input.Password,
				"https://blog-simple.oss-cn-shenzhen.aliyuncs.com/Avatar.jpg", input.Email, input.Phone,
				false);

			await _dbContext.AddAsync(user);
			await _dbContext.SaveChangesAsync();
		}

		[HttpPut]
		public async Task<ResultDto> ChangeRoleAsync([FromBody] ChangeRoleInputDto input)
		{

			await _dbContext.Users.Where(x => x.Id == input.Id).ExecuteUpdateAsync(item =>
				item.SetProperty(x => x.Role, input.Role));

			return new ResultDto();
		}

		[HttpPut]
		public async Task<ResultDto> ChangePasswordAsync([FromBody] ChangePasswordInputDto input)
		{

			var entity = await _dbContext.Users.Where(x => x.Id == _jwtTokenProvider.GetUserId()).FirstOrDefaultAsync();

			if (!entity.CheckCipher(input.Password))
			{
				throw new Exception("Password error");
			}

			entity.SetPassword(input.NewPassword);

			_dbContext.Update(entity);
			await _dbContext.SaveChangesAsync();

			return new ResultDto();
		}

		[HttpPut]
		public async Task<ResultDto> DisableAsync([FromBody] DisableInputDto input)
		{
			if (input.Id == _jwtTokenProvider.GetUserId())
				throw new Exception("You cannot disable yourself");

			// 管理员不能禁用
			await _dbContext.Users.Where(x => x.Id == input.Id && x.Role != RoleType.Admin)
				.ExecuteUpdateAsync(item => item.SetProperty(x => x.IsDisable, input.Disable));


			return new ResultDto();
		}




	}
}
