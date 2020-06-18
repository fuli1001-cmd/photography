using Microsoft.Extensions.Configuration;
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
    public class UserHttpService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;
        private readonly ILogger<UserHttpService> _logger;

        public UserHttpService(HttpClient client, IOptions<ServiceSettings> serviceSettingsOptions, ILogger<UserHttpService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceSettings = serviceSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(serviceSettingsOptions));

            _client.BaseAddress = new Uri(_serviceSettings.UserService);
        }

        public async Task<List<User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var response = await _client.GetAsync($"/api/users/examining?PageNumber={pageNumber}&PageSize={pageSize}");

            response.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<PagedResponseWrapper<List<User>>>(await response.Content.ReadAsStringAsync()).Data;

            users.ForEach(u =>
            {
                u.Avatar = _serviceSettings.FileServer + "/" + u.Avatar;
                u.BackgroundImage = _serviceSettings.FileServer + "/" + u.BackgroundImage;
            });

            return users;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            bool result = false;

            // 更新用户信息
            var updateUserCommand = new 
            { 
                UserId = user.Id, 
                Nickname = user.Nickname,
                Sign = user.Sign,
                Avatar = user.Avatar,
                Gender = user.Gender,
                Birthday = user.Birthday,
                UserType = user.UserType,
                Province = user.Province,
                City = user.City
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(updateUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users", httpContent);
            response.EnsureSuccessStatusCode();

            if (JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data)
            {
                // 更新用户背景图
                var updateBackgroundCommand = new { BackgroundImage = user.BackgroundImage };
                httpContent = new StringContent(JsonConvert.SerializeObject(updateUserCommand), Encoding.UTF8, "application/json");
                response = await _client.PutAsync("/api/users/backgroundimage", httpContent);
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
            }

            return result;
        }
    }
}
