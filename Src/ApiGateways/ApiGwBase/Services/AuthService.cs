using Arise.DDD.API;
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
    public class AuthService
    {
        private readonly HttpClient _client;
        private readonly ServiceSettings _serviceSettings;

        public AuthService(HttpClient client, IOptions<ServiceSettings> serviceSettingsOptions)
        {
            _serviceSettings = serviceSettingsOptions.Value;
            _client = client;
            _client.BaseAddress = new Uri(_serviceSettings.AuthService);
        }

        public async Task<bool> RegisterWithPhoneNumberAsync(string phoneNumber, string code)
        {
            var content = new { PhoneNumber = phoneNumber, VerifyCode = code };
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/register/phone", httpContent);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ResponseWrapper<bool>>(await response.Content.ReadAsStringAsync()).Data;
        }

        public async Task ChangeToRandomPasswordAsync(string phoneNumber, string oldPassword, string newPassword)
        {
            var content = new { UserName = phoneNumber, OldPassword = oldPassword, NewPassword = newPassword };
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/api/register/password/change", httpContent);

            response.EnsureSuccessStatusCode();
        }
    }
}
