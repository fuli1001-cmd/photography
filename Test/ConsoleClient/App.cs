using ConsoleClient.Models;
using IdentityModel.Client;
using IdentityModel.OidcClient;
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
                await GetPostsAsync(await Login());
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

        private async Task<string> Login()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:10001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return string.Empty;
            }

            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ro.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",

                UserName = "4420533@qq.com",
                Password = "Fl1001!@",
                Scope = "openid profile Photography.Post.API Arise.FileUploadService"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return string.Empty;
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }
    }
}
