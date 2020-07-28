using Arise.DDD.Domain.Exceptions;
using IdentityModel.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Photography.Services.User.API.BackwardCompatibility.ChatServerRedis;
using Photography.Services.User.API.BackwardCompatibility.Utils;
using Photography.Services.User.API.BackwardCompatibility.ViewModels;
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
        private readonly IChatServerRedis _chatServerRedisService;
        private readonly IUserRepository _userRepository;

        public LoginCommandHandler(
            IChatServerRedis chatServerRedisService,
            IUserRepository userRepository,
            IOptionsSnapshot<AuthSettings> authSettings,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _authSettings = authSettings ?? throw new ArgumentNullException(nameof(authSettings));
            _chatServerRedisService = chatServerRedisService ?? throw new ArgumentNullException(nameof(chatServerRedisService));
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
                // 更新本次登录的客户端类型和推送id
                user.SetChatServerProperties(request.ClientType, request.RegistrationId);
                await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                // 生成老式token，供聊天服务使用
                oldToken = OldTokenUtil.GetTokenString(user.ChatServerUserId, request.Password);

                // 向redis写入用户信息
                await UpdateRedisAsync(user, oldToken);
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
                throw new ApplicationException("登录失败");

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
                throw new ClientException("用户名或密码错误", new List<string> { tokenResponse.Error, tokenResponse.ErrorDescription });

            return tokenResponse.AccessToken;
        }

        #region BackwardCompatibility: 为了兼容以前的聊天服务，需要向redis写入相关数据
        private async Task UpdateRedisAsync(Domain.AggregatesModel.UserAggregate.User user, string oldToken)
        {
            try
            {
                // 向redis中写入用户信息，供聊天服务使用
                await _chatServerRedisService.WriteUserAsync(user);

                // 向redis中写入老token信息，供聊天服务使用
                await _chatServerRedisService.WriteTokenUserAsync(user, oldToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis Error: {@RedisError}", ex);
            }
        }
        #endregion
    }
}
