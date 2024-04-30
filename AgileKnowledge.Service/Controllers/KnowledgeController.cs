using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Helper;
using AgileKnowledge.Service.Mappings.KnowledgeBases;
using AgileKnowledge.Service.Mappings;
using AgileKnowledge.Service.Mappings.Users;
using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videochatspa.Server.Mappings.BaseDto;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AgileKnowledge.Service.Domain.Enities;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class KnowledgeController : ControllerBase
	{

		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly JwtTokenProvider _jwtTokenProvider;

		public KnowledgeController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_jwtTokenProvider = jwtTokenProvider;
		}

		[HttpGet]
		public async Task<PagedResultDto<KnowledgeBasesDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
		{
			var query = _dbContext.KnowledgeBases.WhereIf(!string.IsNullOrEmpty(input.Filter),
				x => x.Name.Contains(input.Filter));

			var total = query.Count();

			var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

			return new PagedResultDto<KnowledgeBasesDto>(total, _mapper.Map<List<KnowledgeBasesDto>>(list));
		}

		[HttpGet]
		public async Task<KnowledgeBasesDto> GetAsync(Guid id)
		{
			var entity = await _dbContext.KnowledgeBases.Where(x => x.Id == id).FirstOrDefaultAsync();

			return _mapper.Map<KnowledgeBasesDto>(entity);
		}

		[HttpPost]
		public async Task CreateAsync([FromBody] CreateKnowledgeBasesInput input)
		{
			var entity = new KnowledgeBase(input.Icon, input.Name, input.Model, input.EmbeddingModel);

			await _dbContext.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		[HttpPut]
		public async Task UpdateAsync([FromBody] UpdateKnowledgeBasesInput input)
		{
			var entity = await  _dbContext.KnowledgeBases.Where(x => x.Id == input.Id).FirstOrDefaultAsync();

			entity.Name = input.Name;
			entity.Model = input.Model;
			entity.EmbeddingModel = input.EmbeddingModel;
			entity.Icon = input.Icon;

			_dbContext.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		[HttpDelete]
		public async Task DeleteAsync(Guid id)
		{
			var entity =	await _dbContext.KnowledgeBases.Where(x => x.Id == id).Include(x => x.KnowledgeBaseDetails)
				.FirstOrDefaultAsync();

			_dbContext.Remove(entity);

			 await	_dbContext.SaveChangesAsync();
		}




		[HttpPost]
		public async Task CreateDetailsAsync([FromBody] CreateKnowledgeDetailsInput input)
		{
			var fileEntity = await _dbContext.FileStorages.FirstOrDefaultAsync(x => x.Id == input.FileId);
			var knowledgeEntity = await _dbContext.KnowledgeBases.FirstOrDefaultAsync(x => x.Id == input.KnowledgeId);

			var entity = new KnowledgeBaseDetails(fileEntity, knowledgeEntity);
			entity.TrainingPattern = input.TrainingPattern;
			entity.MaxTokensPerLine = input.MaxTokensPerLine;
			entity.MaxTokensPerParagraph = input.MaxTokensPerParagraph;
			entity.OverlappingTokens = input.OverlappingTokens;
			entity.QAPromptTemplate = input.QAPromptTemplate;

			_dbContext.KnowledgeBaseDetails.Add(entity);

			await _dbContext.SaveChangesAsync();
		}

		[HttpDelete]
		public async Task DeleteDetailsAsync(Guid id)
		{
			var entity =	await _dbContext.KnowledgeBaseDetails.Where(x => x.Id == id).FirstOrDefaultAsync();

			_dbContext.Remove(entity);

			await	_dbContext.SaveChangesAsync();
		}

		[HttpGet]
		public async Task<PagedResultDto<KnowledgeBaseDetailsDto>> GetDetailsListAsync([FromQuery] KnowledgeBaseDetailsInputDto input)
		{
			var query = _dbContext.KnowledgeBases.Where(x => x.Id == input.KnowledgeBaseId)
				.Include(x => x.KnowledgeBaseDetails)
				.SelectMany(x => x.KnowledgeBaseDetails).WhereIf(input.State != null,x => x.State == input.State);

			var total = query.Count();

			var list = await query.PageBy(input.PageNumber, input.PageSize).ToListAsync();

			return new PagedResultDto<KnowledgeBaseDetailsDto>(total, _mapper.Map<List<KnowledgeBaseDetailsDto>>(list));
		}









		[HttpGet]
		public async Task<List<CheckQuantizationStateDto>> CheckQuantizationStateAsync(Guid knowledgeBaseId)
		{



			return new List<CheckQuantizationStateDto>();
		}



	}
}
