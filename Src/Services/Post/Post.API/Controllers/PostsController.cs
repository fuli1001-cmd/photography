using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Post.DeletePost;
using Photography.Services.Post.API.Application.Commands.Post.ForwardPosts;
using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using Photography.Services.Post.API.Application.Commands.Post.SharePost;
using Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost;
using Photography.Services.Post.API.Application.Commands.Post.UpdatePost;
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
        /// 帖子列表
        /// </summary>
        /// <param name="key">搜索的关键字，根据用户昵称和帖子文本内容搜索帖子</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> SearchPostsAsync([FromQuery(Name = "key")] string key)
        {
            var posts = await _postQueries.SearchPosts(key, null);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
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
            //_logger.LogInformation("-----UserId: {UserId}-----", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            //_logger.LogInformation("-----UserName: {UserName}-----", User.FindFirst(ClaimTypes.Name)?.Value);
            //_logger.LogInformation("-----Authed: {Authed}-----", User.Identity.IsAuthenticated);
            //_logger.LogInformation("-----AuthType: {AuthType}-----", User.Identity.AuthenticationType);
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
        /// <param name="key">搜索的关键字，根据用户昵称和帖子文本内容搜索帖子</param>
        /// <returns></returns>
        [HttpGet]
        [Route("samecity/{cityCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetSameCityPostsAsync(string cityCode, [FromQuery(Name = "key")] string key)
        {
            List<PostViewModel> posts = null;

            if (string.IsNullOrEmpty(key))
                posts = await _postQueries.GetSameCityPostsAsync(cityCode);
            else
                posts = await _postQueries.SearchPosts(key, cityCode);

            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 用户的帖子
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> GetMyPostsAsync(Guid userId)
        {
            var posts = await _postQueries.GetUserPostsAsync(userId);
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
        /// 获取帖子详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("post/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PostViewModel>> GetPostAsync(Guid postId)
        {
            var post = await _postQueries.GetPostAsync(postId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
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
            return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
        }

        /// <summary>
        /// 更新帖子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PostViewModel>> UpdatePostAsync([FromBody] UpdatePostCommand command)
        {
            var post = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
        }

        /// <summary>
        /// 删除帖子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeletePostAsync([FromBody] DeletePostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 赞或取消赞一个帖子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("togglelike")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ToggleLikePostAsync([FromBody] ToggleLikePostCommand likePostCommand)
        {
            var result = await _mediator.Send(likePostCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 转发帖子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("forward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ForwardPostAsync([FromBody] ForwardPostsCommand forwardPostsCommand)
        {
            var posts = await _mediator.Send(forwardPostsCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 分享帖子
        /// </summary>
        /// <param name="sharePostCommand"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("share")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> ShareAsync([FromBody] SharePostCommand sharePostCommand)
        {
            var result = await _mediator.Send(sharePostCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
