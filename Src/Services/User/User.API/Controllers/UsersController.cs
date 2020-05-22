﻿using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.User.API.Application.Commands.Login;
using Photography.Services.User.API.BackwardCompatibility.ViewModels;
using Photography.Services.User.API.Query.BackwardCompatibility.ViewModels;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserQueries _userQueries;
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;
        private readonly IOptionsSnapshot<ServerSettings> _serverSettings;

        public UsersController(IMediator mediator, IUserQueries userQueries,
            IOptionsSnapshot<ServerSettings> serverSettings, ILogger<UsersController> logger)
        {
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<TokensViewModel>> LoginAsync([FromBody] LoginCommand loginCommand)
        {
            var tokensViewModel = await _mediator.Send(loginCommand);
            if (string.IsNullOrEmpty(tokensViewModel.AccessToken))
            {
                var code = (int)HttpStatusCode.BadRequest;
                return StatusCode(code, ResponseWrapper.CreateErrorResponseWrapper(code, "登录失败"));
            }
            else
                return Ok(ResponseWrapper.CreateOkResponseWrapper(tokensViewModel));
        }

        /// <summary>
        /// 获取用户及配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserViewModel> GetInfoAsync()
        {
            var user = _userQueries.GetCurrentUserAsync();
            var info = new InfoViewModel { UserViewModel = user, ServerSettings = _serverSettings.Value };
            return Ok(ResponseWrapper.CreateOkResponseWrapper(info));
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserViewModel> GetCurrentUserAsync()
        {
            var user = _userQueries.GetCurrentUserAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user));
        }

        /// <summary>
        /// 获取朋友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("friends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<FriendViewModel> GetFriendsAsync()
        {
            var friends = _userQueries.GetFriendsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(friends));
        }
    }
}
