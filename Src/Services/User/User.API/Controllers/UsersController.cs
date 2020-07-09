using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.User.API.Application.Commands.Login;
using Photography.Services.User.API.Application.Commands.Logout;
using Photography.Services.User.API.Application.Commands.User.AllowViewFollowedUsers;
using Photography.Services.User.API.Application.Commands.User.AllowViewFollowers;
using Photography.Services.User.API.Application.Commands.User.AuthRealName;
using Photography.Services.User.API.Application.Commands.User.MuteUser;
using Photography.Services.User.API.Application.Commands.User.ToggleFollow;
using Photography.Services.User.API.Application.Commands.User.UpdateBackground;
using Photography.Services.User.API.Application.Commands.User.UpdateUser;
using Photography.Services.User.API.Application.Commands.User.UploadIdCard;
using Photography.Services.User.API.BackwardCompatibility.ViewModels;
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

        public UsersController(IMediator mediator, IUserQueries userQueries, ILogger<UsersController> logger)
        {
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
            return Ok(ResponseWrapper.CreateOkResponseWrapper(tokensViewModel));
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns>true：退出成功，false：退出失败</returns>
        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> LogoutAsync()
        {
            var command = new LogoutCommand();
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
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
        public async Task<ActionResult<ResponseWrapper>> GetFriendsAsync()
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
        /// 获取用户的关注者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("followers/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetFollowersAsync(Guid userId)
        {
            var followers = await _userQueries.GetFollowersAsync(userId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(followers));
        }

        /// <summary>
        /// 获取用户关注的人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("followedUsers/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetFollowedUsersAsync(Guid userId)
        {
            var followedUsers = await _userQueries.GetFollowedUsersAsync(userId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(followedUsers));
        }

        /// <summary>
        /// 根据昵称搜索用户
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetUserAsync([FromQuery(Name = "key")] string key)
        {
            var users = await _userQueries.SearchUsersAsync(key);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(users));
        }

        /// <summary>
        /// 设置用户免打扰
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("mute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> MuteUserAsync([FromBody] MuteUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 设置是否允许查看我关注的人
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("allowviewfollowedusers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AllowViewFollowedUsersAsync([FromBody] AllowViewFollowedUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 设置是否允许查看关注我的人
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("allowviewfollowers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AllowViewFollowersAsync([FromBody] AllowViewFollowersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 上传身份证照片
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("idcard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UploadIdCardAsync([FromBody] UploadIdCardCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("authrealname")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<bool>> AuthRealNameAsync([FromBody] AuthRealNameCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取用于审核的用户列表
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("examining")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<PagedResponseWrapper>> GetExaminingUsersAsync([FromQuery] PagingParameters pagingParameters)
        {
            var users = await _userQueries.GetExaminingUsersAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(users));
        }
    }
}
