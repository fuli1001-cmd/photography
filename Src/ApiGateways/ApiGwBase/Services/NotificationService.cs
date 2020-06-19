using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Settings;
using Photography.ApiGateways.ApiGwBase.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Services
{
    public class NotificationService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;

        public NotificationService(HttpClient client, IOptions<ServiceSettings> serviceSettingsOptions)
        {
            _serviceSettings = serviceSettingsOptions.Value;
            _client = client;
            _client.BaseAddress = new Uri(_serviceSettings.NotificationService);
        }

        public async Task<UserDto> GetPushSettingsAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/users/pushsettings");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<UserDto>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
