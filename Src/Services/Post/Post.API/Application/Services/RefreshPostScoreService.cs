using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.Post.API.Settings;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Services
{
    public class RefreshPostScoreService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PostScoreRewardSettings _scoreRewardSettings;
        private readonly ILogger<RefreshPostScoreService> _logger;

        private Timer _timer;

        // can not inject IRepository<T> directly, because IRepository<T> is scoped, 
        // while BackgroundService is singleton, have to use IServiceScopeFactory to generate a scope
        public RefreshPostScoreService(
            IServiceScopeFactory serviceScopeFactory,
            IOptionsSnapshot<PostScoreRewardSettings> scoreRewardOptions,
            ILogger<RefreshPostScoreService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _scoreRewardSettings = scoreRewardOptions?.Value ?? throw new ArgumentNullException(nameof(scoreRewardOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async x =>
            {
                _logger.LogInformation("Start refresh post score.");

                await RefreshPostsScoreAsync();

                _logger.LogInformation("End refresh post score.");
            }, null, 0, 60000);
        }

        private async Task RefreshPostsScoreAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();

                try
                {
                    await postRepository.RefreshPostScore(_scoreRewardSettings.StartRefreshHour, _scoreRewardSettings.RefreshIntervalHour, _scoreRewardSettings.Percent);
                }
                catch(Exception ex)
                {
                    _logger.LogError("RefreshPostsScoreAsync: {RefreshPostsScoreException}", ex);
                }
            }
        }
    }
}
