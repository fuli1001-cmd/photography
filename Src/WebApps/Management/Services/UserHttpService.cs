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

        public async Task<PagedResponseWrapper<List<User>>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var response = await _client.GetAsync($"/api/users/examining?PageNumber={pageNumber}&PageSize={pageSize}");

            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<PagedResponseWrapper<List<User>>>(await response.Content.ReadAsStringAsync());

            result.Data.ForEach(u =>
            {
                if (!string.IsNullOrWhiteSpace(u.Avatar))
                    u.Avatar = _serviceSettings.FileServer + "/" + u.Avatar;

                if (!string.IsNullOrWhiteSpace(u.BackgroundImage))
                    u.BackgroundImage = _serviceSettings.FileServer + "/" + u.BackgroundImage;

                if (!string.IsNullOrWhiteSpace(u.IdCardFront))
                    u.IdCardFront = _serviceSettings.FileServer + "/" + u.IdCardFront;

                if (!string.IsNullOrWhiteSpace(u.IdCardBack))
                    u.IdCardBack = _serviceSettings.FileServer + "/" + u.IdCardBack;

                if (!string.IsNullOrWhiteSpace(u.IdCardHold))
                    u.IdCardHold = _serviceSettings.FileServer + "/" + u.IdCardHold;
            });

            return result;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            string avatar = user.Avatar;
            if (!string.IsNullOrWhiteSpace(avatar))
                avatar = avatar.Replace(_serviceSettings.FileServer + "/", string.Empty);

            // 更新用户信息
            var updateUserCommand = new 
            { 
                UserId = user.Id, 
                Nickname = user.Nickname,
                Sign = user.Sign,
                Avatar = avatar,
                Gender = user.Gender,
                Birthday = user.Birthday,
                UserType = user.UserType,
                Province = user.Province,
                City = user.City
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(updateUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users", httpContent);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }

        public async Task<bool> UpdateUserBackgroundAsync(User user)
        {
            string backgroundImage = user.BackgroundImage;
            if (!string.IsNullOrWhiteSpace(backgroundImage))
                backgroundImage = backgroundImage.Replace(_serviceSettings.FileServer + "/", string.Empty);

            var command = new { BackgroundImage = backgroundImage, UserId = user.Id };
            var httpContent = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users/backgroundimage", httpContent);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }

        public async Task<bool> AuthRealNameAsync(User user, bool passed)
        {
            var command = new
            {
                UserId = user.Id,
                Passed = passed
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users/authrealname", httpContent);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }

        public async Task<bool> DisableUserAsync(User user, bool disabled)
        {
            var command = new
            {
                UserId = user.Id,
                Disabled = disabled
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users/disable", httpContent);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PagedResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
