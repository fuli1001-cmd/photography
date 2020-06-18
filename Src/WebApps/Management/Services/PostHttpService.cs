using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.WebApps.Management.Settings;
using Photography.WebApps.Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.Services
{
    public class PostHttpService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;
        private readonly ILogger<PostHttpService> _logger;

        public PostHttpService(HttpClient client, IOptions<ServiceSettings> serviceSettingsOptions, ILogger<PostHttpService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceSettings = serviceSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(serviceSettingsOptions));

            _client.BaseAddress = new Uri(_serviceSettings.PostService);
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId, int pageNumber, int pageSize)
        {
            var response = await _client.GetAsync($"/api/posts/user/{userId}?PageNumber={pageNumber}&PageSize={pageSize}");

            response.EnsureSuccessStatusCode();

            var posts = JsonConvert.DeserializeObject<PagedResponseWrapper<List<Post>>>(await response.Content.ReadAsStringAsync()).Data;

            return posts;
        }
    }
}
