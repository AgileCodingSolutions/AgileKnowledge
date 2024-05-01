﻿using AgileKnowledge.Service.Domain;
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
using AgileKnowledge.Service.Service;
using Microsoft.KernelMemory;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class KnowledgeController : ControllerBase
	{

		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly JwtTokenProvider _jwtTokenProvider;
		private readonly KnowledgeMemoryService _memoryService;

		public KnowledgeController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider, KnowledgeMemoryService memoryService)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_jwtTokenProvider = jwtTokenProvider;
			_memoryService = memoryService;
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
		public async Task<PagedResultDto<KnowledgeBaseDetailVectorQuantityDto>> GetDetailVectorQuantityAsync(KnowledgeBaseDetailsVectorQuantityInputDto input)
		{

			var memoryServerless = _memoryService.CreateMemoryServerless();
			var memoryDbs = memoryServerless.Orchestrator.GetMemoryDbs();

			var entity = await _dbContext.KnowledgeBaseDetails.Where(x=>x.Id == input.KnowledgeBaseDetailsId).FirstOrDefaultAsync();

			var dto = new List<KnowledgeBaseDetailVectorQuantityDto>();

			foreach (var memoryDb in memoryDbs)
			{
				// 通过pageSize和page获取到最大数量
				var limit = input.PageSize * input.PageNumber;
				if (limit < 10)
				{
					limit = 10;
				}

				var filter = new MemoryFilter().ByDocument(input.KnowledgeBaseDetailsId.ToString());

				int size = 0;
				await foreach (var item in memoryDb.GetListAsync("wiki", new List<MemoryFilter>()
				               {
					               filter
				               }, limit, true))
				{
					size++;
					if (size < input.PageSize * (input.PageNumber - 1))
					{
						continue;
					}

					if (size > input.PageSize * input.PageNumber)
					{
						break;
					}

					dto.Add(new KnowledgeBaseDetailVectorQuantityDto()
					{
						Content = item.Payload["text"].ToString() ?? string.Empty,
						FileId = item.Tags.FirstOrDefault(x => x.Key == "fileId").Value?.FirstOrDefault() ?? string.Empty,
						Id = item.Id,
						Index = size,
						WikiDetailId = item.Tags["wikiDetailId"].FirstOrDefault() ?? string.Empty,
						Document_Id = item.Tags["__document_id"].FirstOrDefault() ?? string.Empty
					});
				}
			}
			

			return new PagedResultDto<KnowledgeBaseDetailVectorQuantityDto>(entity.DataCount, dto);
		}


		[HttpGet]
		public async Task<List<CheckQuantizationStateDto>> CheckQuantizationStateAsync(Guid knowledgeBaseId)
		{

			var values = QuantizeBackgroundService.CacheKnowledgeBaseDetails.Values.Where(x => x.Item1.KnowledgeBase.Id == knowledgeBaseId).ToList();

			if (values.Any())
			{
				return values.Select(x => new CheckQuantizationStateDto
				{
					KnowledgeBaseDetailsId = x.Item1.Id,
					FileName = x.Item1.File.FullName,
					State = x.Item1.State
				}).ToList();
			}

			return new List<CheckQuantizationStateDto>();
		}



	}
}
