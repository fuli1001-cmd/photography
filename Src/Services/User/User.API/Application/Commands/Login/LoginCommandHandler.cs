using IdentityModel.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.API.BackwardCompatibility.Utils;
using Photography.Services.User.API.BackwardCompatibility.ViewModels;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.API.Settings;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokensViewModel>
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IOptionsSnapshot<AuthSettings> _authSettings;
        private readonly IRedisService _redisService;
        private readonly IUserRepository _userRepository;

        public LoginCommandHandler(IRedisService redisService,
            IUserRepository userRepository,
            IOptionsSnapshot<AuthSettings> authSettings,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _authSettings = authSettings ?? throw new ArgumentNullException(nameof(authSettings));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TokensViewModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync(request);
            string oldToken = null;

            #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            if (user != null)
            {
                // 生成老式token，供聊天服务使用
                oldToken = OldTokenUtil.GetTokenString(user.ChatServerUserId, request.Password);
                // 向redis中写入用户信息，供聊天服务使用
                await WriteChatServerUserToRedisAsync(request, user);
                // 向redis中写入老token信息，供聊天服务使用
                await WriteTokenToRedisAsync(request, user, oldToken);
            }
            #endregion

            return new TokensViewModel { AccessToken = accessToken, OldToken = oldToken };
        }

        private async Task<string> GetAccessTokenAsync(LoginCommand loginCommand)
        {
            // discover endpoints from metadata
            var httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _authSettings.Value.Authority,
                Policy =
                {
                    RequireHttps = false
                }
            });
            if (disco.IsError)
            {
                _logger.LogError("{LoginError}", disco.Error);
                return null;
            }

            var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _authSettings.Value.ClientId,
                ClientSecret = _authSettings.Value.ClientSecret,
                Scope = _authSettings.Value.Scope,
                UserName = loginCommand.UserName,
                Password = loginCommand.Password
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError("{LoginError}", tokenResponse.ErrorDescription);
                return null;
            }

            return tokenResponse.AccessToken;
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task WriteChatServerUserToRedisAsync(LoginCommand loginCommand, Domain.AggregatesModel.UserAggregate.User user)
        {
            try
            {
                var chatServerUser = new UserInfoLite
                {
                    userId = user.ChatServerUserId,
                    username = user.UserName,
                    nickname = user.Nickname,
                    clientType = loginCommand.ClientType,
                    avatar = user.Avatar,
                    tel = user.Phonenumber,
                    password = loginCommand.Password,
                    registrationId = loginCommand.RegistrationId
                };
                _logger.LogInformation("UserInfoLite: {@UserInfoLite}", chatServerUser);
                string json = SerializeUtil.SerializeToJson(chatServerUser);
                var bytes = SerializeUtil.SerializeStringToBytes(json, true);
                json = JsonConvert.SerializeObject(bytes);
                await _redisService.StringSetAsync(user.ChatServerUserId.ToString(), json, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteChatServerUserToRedisAsync: {BackwardCompatibilityError}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("WriteChatServerUserToRedisAsync: {BackwardCompatibilityError}", ex.InnerException.Message);
            }
        }

        private async Task WriteTokenToRedisAsync(LoginCommand loginCommand, Domain.AggregatesModel.UserAggregate.User user, string oldToken)
        {
            try
            {
                var token = new Token
                {
                    userId = user.ChatServerUserId,
                    username = user.UserName,
                    nickname = user.Nickname,
                    clientType = loginCommand.ClientType,
                    loginTime = CommonUtil.GetTimestamp(DateTime.Now)
                };
                _logger.LogInformation("TokenUser: {@TokenUser}", token);
                var bytes = SerializeUtil.SerializeToJsonBytes(token, true);
                var json = JsonConvert.SerializeObject(bytes);
                await _redisService.StringSetAsync(oldToken, json, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("WriteTokenToRedisAsync: {BackwardCompatibilityError}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("WriteTokenToRedisAsync: {BackwardCompatibilityError}", ex.InnerException.Message);
            }
        }
        #endregion
    }
}
