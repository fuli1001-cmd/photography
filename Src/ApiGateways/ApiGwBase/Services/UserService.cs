using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Services
{
    public class UserService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;

        public UserService(HttpClient client, IOptions<ServiceSettings> serviceSettingsOptions)
        {
            _serviceSettings = serviceSettingsOptions.Value;
            _client = client;
            _client.BaseAddress = new Uri(_serviceSettings.UserService);
        }

        public async Task<TokensViewModel> LoginWithPhoneNumberAsync(string phoneNumber, string code, int clientType, string registrationId)
        {
            var content = new { UserName = phoneNumber, Password = code, ClientType = clientType, RegistrationId = registrationId };
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/users/login", httpContent);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<TokensViewModel>>(await response.Content.ReadAsStringAsync()).Data;
        }
    }
}
