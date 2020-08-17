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
    public class PostService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;

        public PostService(HttpClient client, IOptions<ServiceSettings> serviceOptions)
        {
            _serviceSettings = serviceOptions?.Value ?? throw new ArgumentNullException(nameof(serviceOptions));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri(_serviceSettings.PostService);
        }

        /// <summary>
        /// 获取我发出的约拍交易数量
        /// </summary>
        /// <returns></returns>
        public async Task<UnReadEventCountDto> GetSentAndReceivedAppointmentDealCountAsync()
        {
            var response = await _client.GetAsync($"/api/AppointmentDeals/SentAndReceivedAppointmentDealCount");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<UnReadEventCountDto>>(await response.Content.ReadAsStringAsync()).Data;
        }

        /// <summary>
        /// 获取系统帖子标签
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetSystemTagsAsync()
        {
            var response = await _client.GetAsync($"/api/tags/system");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<IEnumerable<string>>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
