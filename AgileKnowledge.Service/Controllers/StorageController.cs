using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Helper;
using AgileKnowledge.Service.Mappings.Storage;
using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgileKnowledge.Service.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class StorageController : ControllerBase
	{
		private readonly KnowledgeDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly JwtTokenProvider _jwtTokenProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StorageController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_jwtTokenProvider = jwtTokenProvider;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpPost]
		public async Task<ActionResult<UploadFileResult>> UploadFile(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("File is not selected");
			}

			var filePath = "uploads/" + DateTime.Now.ToString("yyyyMMdd") + "/" + Guid.NewGuid().ToString("N") + file.FileName;
			var fileStreamPath = Path.Combine("wwwroot", filePath);

			var fileInfo = new FileInfo(fileStreamPath);

			if (fileInfo.Directory?.Exists == false)
			{
				fileInfo.Directory.Create();
			}

			try
			{
				await using var fileStream = fileInfo.Create();
				await file.CopyToAsync(fileStream);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving file: {ex.Message}");
			}

			var fileStorage = new FileStorage(file.FileName, filePath,"file", file.Length, false);
	
			await _dbContext.FileStorages.AddAsync(fileStorage);
			await _dbContext.SaveChangesAsync();

			return new UploadFileResult
			{
				Id = fileStorage.Id,
				Path = fileStorage.Path
			};
		}


		[HttpPost]
		public async Task<ActionResult<UploadFileResult>> UploadWeb(string path)
		{

			var fileStorage = new FileStorage("Html", path,"web", 0, false);
			await _dbContext.FileStorages.AddAsync(fileStorage);
			await _dbContext.SaveChangesAsync();

			return new UploadFileResult
			{
				Id = fileStorage.Id,
				Path = fileStorage.Path
			};
		}

		[HttpPost]
		public async Task<ActionResult<UploadFileResult>> UploadData(string path)
		{
			
			var fileStorage = new FileStorage("Data", path,"data", 0, false);
			await _dbContext.FileStorages.AddAsync(fileStorage);
			await _dbContext.SaveChangesAsync();

			return new UploadFileResult
			{
				Id = fileStorage.Id,
				Path = fileStorage.Path
			};
		}

	}
}
