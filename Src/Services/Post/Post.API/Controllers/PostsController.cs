using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.LikePost;
using Photography.Services.Post.API.Application.Commands.PublishPost;
using Photography.Services.Post.API.Infrastructure;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    /// <summary>
    /// 帖子控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostQueries _postQueries;
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator, IPostQueries postQueries, ILogger<PostsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取热门帖子列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("hot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetHotPostsAsync()
        {
            _logger.LogInformation("-----UserId: {UserId}-----", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _logger.LogInformation("-----UserName: {UserName}-----", User.FindFirst(ClaimTypes.Name)?.Value);
            _logger.LogInformation("-----Authed: {Authed}-----", User.Identity.IsAuthenticated);
            _logger.LogInformation("-----AuthType: {AuthType}-----", User.Identity.AuthenticationType);
            var posts = await _postQueries.GetHotPostsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 获取关注用户的帖子列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("followed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetFollowedPostsAsync()
        {
            var posts = await _postQueries.GetFollowedPostsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 获取同城帖子列表
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("samecity/{cityCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetSameCityPostsAsync(string cityCode)
        {
            var posts = await _postQueries.GetSameCityPostsAsync(cityCode);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 我的帖子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetMyPostsAsync()
        {
            var posts = await _postQueries.GetMyPostsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 我赞过的帖子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("likes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetLikedPostsAsync()
        {
            var posts = await _postQueries.GetLikedPostsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 创建帖子
        /// </summary>
        /// <param name="publishPostCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PostViewModel>> PublishPostAsync([FromBody] PublishPostCommand publishPostCommand)
        {
            var post = await _mediator.Send(publishPostCommand);
            return StatusCode((int)HttpStatusCode.Created, ResponseWrapper.CreateOkResponseWrapper(post));
        }

        /// <summary>
        /// 赞或取消赞一个帖子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("togglelike")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PostViewModel>> LikePostAsync([FromBody] ToggleLikePostCommand likePostCommand)
        {
            var result = await _mediator.Send(likePostCommand);
            return StatusCode((int)HttpStatusCode.Created, ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
