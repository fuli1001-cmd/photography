using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.WebApps.Management.Settings;
using Photography.WebApps.Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public async Task<PagedResponseWrapper<List<Post>>> GetPostsAsync(int pageNumber, int pageSize)
        {
            return await DoGetPostsAsync("posts", pageNumber, pageSize);
        }

        public async Task<PagedResponseWrapper<List<Post>>> GetAppointsAsync(int pageNumber, int pageSize)
        {
            return await DoGetPostsAsync("appointments", pageNumber, pageSize);
        }

        public async Task<bool> DeletePostAsync(Post post)
        {
            return await DoDeletePostAsync("posts",post);
        }

        public async Task<bool> DeleteAppointmentAsync(Post post)
        {
            return await DoDeletePostAsync("appointments", post);
        }

        private async Task<PagedResponseWrapper<List<Post>>> DoGetPostsAsync(string type, int pageNumber, int pageSize)
        {
            var response = await _client.GetAsync($"/api/{type}?PageNumber={pageNumber}&PageSize={pageSize}");

            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<PagedResponseWrapper<List<Post>>>(await response.Content.ReadAsStringAsync());

            result.Data.ForEach(post =>
            {
                post.PostAttachments.ForEach(attachment =>
                {
                    attachment.Name = _serviceSettings.FileServer + "/" + attachment.Name;
                });
            });

            return result;
        }

        private async Task<bool> DoDeletePostAsync(string type, Post post)
        {
            var command = new { PostId = post.Id, AppointmentId = post.Id, UserId = post.User.Id };
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress, $"/api/{type}")
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
