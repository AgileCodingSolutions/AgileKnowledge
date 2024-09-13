using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Helper;
using AgileKnowledge.Service.Mappings;
using AgileKnowledge.Service.Mappings.FastWikiFunction;
using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Mvc;

namespace AgileKnowledge.Service.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly KnowledgeDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly JwtTokenProvider _jwtTokenProvider;

        public FunctionController(KnowledgeDbContext dbContext, IMapper mapper, JwtTokenProvider jwtTokenProvider)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _jwtTokenProvider = jwtTokenProvider;
        }
        [HttpPost]
        public async Task CreateFunctionAsync(FastWikiFunctionCallInput input)
        {
            var functionCall = new FastWikiFunctionCall()
            {
                Content = input.Content,
                Description = input.Description,
                Imports = input.Imports,
                Enable = true,
                Main = input.Main,
                Items = input.Items.Select(x => new FunctionItem
                {
                    Key = x.Key,
                    Value = x.Value
                }).ToList(),
                Name = input.Name,
                Parameters = input.Parameters.Select(x => new FunctionItem
                {
                    Key = x.Key,
                    Value = x.Value
                }).ToList()
            };
            _dbContext.AddAsync(functionCall);
            await _dbContext.SaveChangesAsync();
        }
    }
}
