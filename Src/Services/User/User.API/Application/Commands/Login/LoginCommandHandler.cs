﻿using IdentityModel.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.User.API.Infrastructure.Redis;
using Photography.Services.User.API.Settings;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
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

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            //var accessToken = await GetAccessTokenAsync(request);
            //if (!string.IsNullOrEmpty(accessToken))
            //{
            //    var user = await _userRepository.GetByUserNameAsync(request.UserName);
            //    if (user != null)
            //    {
            //        var chatServerToken = GetChatServerToken(user.Id);
            //    }
            //}
            throw new NotImplementedException();
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

        private string GetChatServerToken(int userId, string password)
        {
            //var content = userId + "_" + password + "_" + CommonUtil.GetTimestamp(DateTime.Now);
            //return Encryptor.EncryptDES(content, ENCRYPT_KEY);
            return null;
        }
    }
}
