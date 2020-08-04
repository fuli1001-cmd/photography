using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Services
{
    public class NotificationService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;

        public NotificationService(HttpClient client, IOptions<ServiceSettings> serviceOptions)
        {
            _serviceSettings = serviceOptions?.Value ?? throw new ArgumentNullException(nameof(serviceOptions));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri(_serviceSettings.NotificationService);
        }

        /// <summary>
        /// 获取我的未读事件数量
        /// </summary>
        /// <returns></returns>
        public async Task<UnReadEventCountDto> GetUnReadEventCountAsync()
        {
            var response = await _client.GetAsync($"/api/events/unread-count");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<UnReadEventCountDto>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
