using ConsoleClient.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class App : BackgroundService
    {
        private readonly PostService _postService;
        private readonly ILogger<App> _logger;

        private string _authority = "https://localhost:10001";

        public App(PostService postService, ILogger<App> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //var loginResult = await Logout();
                //var loginResult = await Login();
                //ShowLoginResult(loginResult);
                //await ConnectSignalHubAsync(loginResult.AccessToken);
                await GetPostsAsync("");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task GetPostsAsync(string accessToken)
        {
            var posts = await _postService.GetPosts(accessToken);
            _logger.LogDebug("{@Posts}", posts);
        }
    }
}
