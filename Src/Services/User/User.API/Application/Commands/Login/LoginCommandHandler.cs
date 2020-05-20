using IdentityModel.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

            #region Backward compatibility code
            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            // 生成老式token，供聊天服务使用
            var oldToken = OldTokenUtil.GetTokenString(user.ChatServerUserId, request.Password);
            // 向redis中写入用户信息，供聊天服务使用
            await WriteChatServerUserToRedisAsync(request, user);
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

        private async Task WriteChatServerUserToRedisAsync(LoginCommand loginCommand, Domain.AggregatesModel.UserAggregate.User user)
        {
            try
            {
                if (user != null)
                {
                    var chatServerUser = new ChatServerUser
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
                    string json = SerializeUtil.SerializeToJson(chatServerUser);
                    await _redisService.SetAsync(user.ChatServerUserId.ToString(), SerializeUtil.SerializeStringToBytes(json, true));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("WriteChatServerUserToRedisAsync: {ChatServerError}", ex.Message);
            }
        }
    }
}
