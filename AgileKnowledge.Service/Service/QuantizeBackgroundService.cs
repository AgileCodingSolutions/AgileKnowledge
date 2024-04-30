using System.Collections.Concurrent;
using System.Threading.Channels;
using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Domain.Enum;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AgileKnowledge.Service.Service
{
	public class QuantizeBackgroundService: BackgroundService
	{

		private readonly ILogger<QuantizeBackgroundService> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly KnowledgeDbContext _dbContext;


		public static ConcurrentDictionary<string, (KnowledgeBaseDetails, KnowledgeBase)> CacheKnowledgeBaseDetails { get; } = new();
		private static int _maxTask = 1;


		private static int _currentTask;
		private static readonly Channel<KnowledgeBaseDetails> KnowledgeBaseDetails = Channel.CreateBounded<KnowledgeBaseDetails>(
			new BoundedChannelOptions(1000)
			{
				SingleReader = true,
				SingleWriter = false
			});


		public QuantizeBackgroundService(IServiceProvider serviceProvider, ILogger<QuantizeBackgroundService> logger, KnowledgeDbContext dbContext)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_dbContext = dbContext;
		}


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var QUANTIZE_MAX_TASK = Environment.GetEnvironmentVariable("QUANTIZE_MAX_TASK");
			if (!string.IsNullOrEmpty(QUANTIZE_MAX_TASK))
			{
				int.TryParse(QUANTIZE_MAX_TASK, out _maxTask);
			}
			if (_maxTask < 0)
			{
				_maxTask = 1;
			}


			await LoadingKnowledgeBaseDetailsAsync();




			var tasks = new List<Task>();
			for (var i = 0; i < _maxTask; i++)
			{
				tasks.Add(Task.Factory.StartNew(KnowledgeBaseDetailsHandlerAsync, stoppingToken));
			}

			await Task.WhenAll(tasks);
		}



		private async Task KnowledgeBaseDetailsHandlerAsync()
		{
			using var asyncServiceScope = _serviceProvider.CreateScope();
			while (await KnowledgeBaseDetails.Reader.WaitToReadAsync())
			{
				Interlocked.Increment(ref _currentTask);
				_logger.LogInformation($"当前任务数量：{_currentTask}");
				var wikiDetail = await KnowledgeBaseDetails.Reader.ReadAsync();
				await HandlerAsync(wikiDetail, asyncServiceScope.ServiceProvider);
				Interlocked.Decrement(ref _currentTask);
			}
		}

		private async ValueTask HandlerAsync(KnowledgeBaseDetails knowledgeBaseDetails, IServiceProvider service)
		{


		}

		private async Task LoadingKnowledgeBaseDetailsAsync()
		{
			using var asyncServiceScope = _serviceProvider.CreateScope();
			var dbContext = asyncServiceScope.ServiceProvider.GetRequiredService<KnowledgeDbContext>();
			var mapper = asyncServiceScope.ServiceProvider.GetRequiredService<IMapper>();
			foreach (var knowledgeBaseDetail in await dbContext.KnowledgeBaseDetails.Where(x=>x.State == KnowledgeBaseQuantizationState.Failed || x.State == KnowledgeBaseQuantizationState.None).ToListAsync())
			{
				await AddKnowledgeBaseDetailAsync(knowledgeBaseDetail);
			}
		}

		public static async Task AddKnowledgeBaseDetailAsync(KnowledgeBaseDetails knowledgeBaseDetail)
		{
			await KnowledgeBaseDetails.Writer.WriteAsync(knowledgeBaseDetail);
		}


	}
}
