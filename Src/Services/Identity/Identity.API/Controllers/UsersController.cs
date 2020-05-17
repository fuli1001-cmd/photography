using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Identity.API.Query.Interfaces;
using Photography.Services.Identity.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Identity.API.Controllers
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
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserViewModel> GetCurrentUserAsync()
        {
            var user = _userQueries.GetCurrentUserAsync();
            return Ok(user);
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
            return Ok(friends);
        }
    }
}
