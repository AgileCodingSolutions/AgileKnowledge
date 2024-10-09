using System;

using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Helper;
using AgileKnowledge.Service.Mappings;
using AgileKnowledge.Service.Mappings.ChatApplications;
using AgileKnowledge.Service.Mappings.KnowledgeBases;
using AgileKnowledge.Service.Migrations;
using AgileKnowledge.Service.Service;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videochatspa.Server.Mappings.BaseDto;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ChatApplicationController : ControllerBase
	{

		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly JwtTokenProvider _jwtTokenProvider;

		public ChatApplicationController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_jwtTokenProvider = jwtTokenProvider;
		}


		[HttpGet]
		public async Task<PagedResultDto<ChatApplicationDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
		{
			var query = _dbContext.ChatApplications.WhereIf(!string.IsNullOrEmpty(input.Filter),
				x => x.Name.Contains(input.Filter));

			var total = query.Count();

			var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

			return new PagedResultDto<ChatApplicationDto>(total, _mapper.Map<List<ChatApplicationDto>>(list));
		}

		[HttpGet]
		public async Task<ChatApplicationDto> GetAsync(Guid id)
		{
			var entity = await _dbContext.ChatApplications.Where(x => x.Id == id)
				.Include(x => x.KnowledgeBases).FirstOrDefaultAsync();

            List<Guid> ids = new List<Guid>();
            if (entity!=null) 
			{
                foreach (var item in entity.KnowledgeBases)
                {
					ids.Add(item.Id);
                }
            }
	
            var dto = _mapper.Map<ChatApplicationDto>(entity) ;
			dto.KnowledgeIds = ids;
			return dto;
        }

		[HttpPost]
		public async Task CreateAsync([FromBody] CreateChatApplicationInputDto input)
		{
			var entity = new ChatApplication(input.Name);

			await _dbContext.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		[HttpPut]
		public async Task UpdateAsync([FromBody] UpdateChatApplicationInputDto input)
		{
			var entity = await  _dbContext.ChatApplications.Where(x => x.Id == input.Id).FirstOrDefaultAsync();

			entity.Name = input.Name;
			entity.Prompt = input.Prompt;
			entity.ChatModel = input.ChatModel;
			entity.Temperature = input.Temperature;
			entity.MaxResponseToken = input.MaxResponseToken;
			entity.Template = input.Template;
			entity.Opener = input.Opener;
			entity.KnowledgeBases =
				await _dbContext.KnowledgeBases.Where(x => input.KnowledgeIds.Contains(x.Id)).ToListAsync();
			
			_dbContext.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		[HttpDelete]
		public async Task DeleteAsync(Guid id)
		{
			var entity =	await _dbContext.ChatApplications.Where(x => x.Id == id).Include(x => x.ChatDialogs)
				.FirstOrDefaultAsync();

			_dbContext.Remove(entity);

			await _dbContext.SaveChangesAsync();
		}




		[HttpPost]
		public async Task CreateChatDialogAsync(CreateChatDialogInputDto input)
		{
			var chatApplication = await _dbContext.ChatApplications.Where(x => x.Id == input.ApplicationId)
				.FirstOrDefaultAsync();

			var entity = new ChatDialog(input.Name, chatApplication, input.Description);

			await _dbContext.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		[HttpGet]
		public async Task<List<ChatDialogDto>> GetChatDialogAsync(string applicationId)
		{
			var entityList = await _dbContext.ChatDialogs.Where(x => x.ChatApplication.Id == Guid.Parse(applicationId)).ToListAsync();

			var data = _mapper.Map<List<ChatDialogDto>>(entityList);

			return data;
		}




		[HttpPost]
		public async Task CreateChatDialogHistoryAsync(CreateChatDialogHistoryInputDto input)
		{
			var chatDialog = await _dbContext.ChatDialogs.Where(x => x.Id == input.ChatDialogId).FirstOrDefaultAsync();


			var chatDialogHistory = new ChatDialogHistory(chatDialog,
				input.Content, TokenHelper.ComputeToken(input.Content),
				input.Type);

		 	await _dbContext.AddAsync(chatDialogHistory);
			await _dbContext.SaveChangesAsync();
		}



		[HttpGet]
		public async Task<PagedResultDto<PostShareDto>> GetPostShareListAsync([FromQuery] PostShareList input)
		{
            if (input.ChatApplicationId == Guid.Empty)
            {
                throw new ArgumentException("Chat application ID cannot be empty.", nameof(input.ChatApplicationId));
            }
            var query = _dbContext.ChatDialogs.WhereIf(!input.ChatApplicationId.Equals(Guid.Empty),
                x => x.Id == input.ChatApplicationId);
            var total = query.Count();
            var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

            return new PagedResultDto<PostShareDto>(total, _mapper.Map<List<PostShareDto>>(list));
        }


		[HttpPost]
		public async Task CreatePostShareAsync([FromBody] CreatePostShareInput input)
		{
            var entity = new CreatePostShareCommand(input.Name, input.ChatApplicationId,input.Expires,input.AvailableToken,input.AvailableQuantity);

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

		[HttpDelete]
		public async Task DeleteChatDialogAsync(Guid id)
		{
            var entity = await _dbContext.ChatDialogs.Where(x => x.Id == id).FirstOrDefaultAsync();

            _dbContext.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }


		[HttpGet]
        public async Task<PagedResultDto<CreateChatDialogHistoryInputDto>> GetChatDialogHistoryAsync([FromQuery] ChatAppHistory input)
        {
            if (input.ChatDialogId == Guid.Empty)
            {
                throw new ArgumentException("Chat application ID cannot be empty.", nameof(input.ChatDialogId));
            }
            var query = _dbContext.ChatDialogHistorys.WhereIf(!input.ChatDialogId.Equals(Guid.Empty),
                x => x.ChatDialog.Id == input.ChatDialogId);

            var total = query.Count();

            var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

			return new PagedResultDto<CreateChatDialogHistoryInputDto>(total, _mapper.Map<List<CreateChatDialogHistoryInputDto>>(list));
        }


        [HttpDelete]
        public async Task DeleteChatDialogHistoryAsync(Guid chatDialohId)
        {
            var entity = await _dbContext.ChatDialogHistorys.Where(x => x.ChatDialog.Id == chatDialohId).FirstOrDefaultAsync();

            _dbContext.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }


    }
}
