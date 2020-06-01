using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.User.API.Application.Commands.Login;
using Photography.Services.User.API.Application.Commands.ToggleFollow;
using Photography.Services.User.API.Application.Commands.UpdateBackground;
using Photography.Services.User.API.Application.Commands.UpdateUser;
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
        /// 获取当前用户及配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MeViewModel>> GetInfoAsync()
        {
            var user = await _userQueries.GetCurrentUserAsync();
            var info = new InfoViewModel { MeViewModel = user, ServerSettings = _serverSettings.Value };
            return Ok(ResponseWrapper.CreateOkResponseWrapper(info));
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MeViewModel>> GetCurrentUserAsync()
        {
            var user = await _userQueries.GetCurrentUserAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<UserViewModel>> GetUserAsync([FromQuery(Name = "userId")] Guid? userId, [FromQuery(Name = "oldUserId")] int? oldUserId, [FromQuery(Name = "nickName")] string nickName)
        {
            var user = await _userQueries.GetUserAsync(userId, oldUserId, nickName);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user));
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateUserAsync([FromBody] UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 更新背景图
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("backgroundimage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateBackgroundImageAsync([FromBody] UpdateBackgroundCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取朋友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("friends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FriendViewModel>> GetFriendsAsync()
        {
            var friends = await _userQueries.GetFriendsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(friends));
        }

        /// <summary>
        /// 关注或取消关注
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("togglefollow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ToggleFollowAsync([FromBody] ToggleFollowCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取当前用户的关注者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("followers/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FriendViewModel>> GetFollowersAsync(Guid userId)
        {
            var followers = await _userQueries.GetFollowersAsync(userId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(followers));
        }

        /// <summary>
        /// 获取当前用户关注的人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("followedUsers/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FriendViewModel>> GetFollowedUsersAsync(Guid userId)
        {
            var followedUsers = await _userQueries.GetFollowedUsersAsync(userId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(followedUsers));
        }
    }
}
