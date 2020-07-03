using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using Arise.DDD.Domain.Exceptions;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Post.DeletePost;
using Photography.Services.Post.API.Application.Commands.Post.ForwardPosts;
using Photography.Services.Post.API.Application.Commands.Post.MarkGoodPost;
using Photography.Services.Post.API.Application.Commands.Post.MovePostOutFromCircle;
using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using Photography.Services.Post.API.Application.Commands.Post.Share;
using Photography.Services.Post.API.Application.Commands.Post.ToggleLikePost;
using Photography.Services.Post.API.Application.Commands.Post.UpdatePost;
using Photography.Services.Post.API.Infrastructure;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        private const string _decryptKey = "Vs16.5.4";

        public PostsController(IMediator mediator, IPostQueries postQueries, Microsoft.Extensions.Configuration.IConfiguration configuration, ILogger<PostsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _postQueries = postQueries ?? throw new ArgumentNullException(nameof(postQueries));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 帖子列表
        /// </summary>
        /// <param name="key">搜索的关键字，根据用户昵称和帖子文本内容搜索帖子</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> SearchPostsAsync([FromQuery(Name = "key")] string key, [FromQuery] PagingParameters pagingParameters)
        {
            var posts = await _postQueries.SearchPosts(key, null, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 获取热门帖子列表
        /// </summary>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("hot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetHotPostsAsync([FromQuery] PagingParameters pagingParameters)
        {
            //_logger.LogInformation("-----UserId: {UserId}-----", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            //_logger.LogInformation("-----UserName: {UserName}-----", User.FindFirst(ClaimTypes.Name)?.Value);
            //_logger.LogInformation("-----Authed: {Authed}-----", User.Identity.IsAuthenticated);
            //_logger.LogInformation("-----AuthType: {AuthType}-----", User.Identity.AuthenticationType);
            var posts = await _postQueries.GetHotPostsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 获取关注用户的帖子列表
        /// </summary>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("followed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetFollowedPostsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var posts = await _postQueries.GetFollowedPostsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 获取同城帖子列表
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <param name="key">搜索的关键字，根据用户昵称、帖子文本及标签搜索帖子</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("samecity/{cityCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetSameCityPostsAsync(string cityCode, [FromQuery(Name = "key")] string key, [FromQuery] PagingParameters pagingParameters)
        {
            PagedList<PostViewModel> posts = null;

            if (string.IsNullOrWhiteSpace(key))
                posts = await _postQueries.GetSameCityPostsAsync(cityCode, pagingParameters);
            else
                posts = await _postQueries.SearchPosts(key, cityCode, pagingParameters);

            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 用户的帖子
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="privateTag">帖子标签</param>
        /// <param name="key">搜索的关键字，根据用户昵称、帖子文本及标签搜索帖子</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetMyPostsAsync(Guid userId, [FromQuery(Name = "privateTag")] string privateTag, [FromQuery(Name = "key")] string key, [FromQuery] PagingParameters pagingParameters)
        {
            var posts = await _postQueries.GetUserPostsAsync(userId, privateTag, key, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 圈子内的帖子
        /// </summary>
        /// <param name="circleId">圈子id</param>
        /// <param name="type">查询类型：good - 精华，hot - 热门， newest - 最新</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("circle/{circleId}/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetCirclePostsAsync(Guid circleId, string type, [FromQuery(Name = "key")] string key, [FromQuery] PagingParameters pagingParameters)
        {
            PagedList<PostViewModel> posts = null;

            type = type.ToLower();
            if (type == "good")
                posts = await _postQueries.GetCirclePostsAsync(circleId, true, key, string.Empty, pagingParameters);
            else if (type == "hot")
                posts = await _postQueries.GetCirclePostsAsync(circleId, false, key, "score", pagingParameters);
            else if (type == "newest")
                posts = await _postQueries.GetCirclePostsAsync(circleId, false, key, string.Empty, pagingParameters);

            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 设置帖子为精华或取消设置为精华
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("circlegood")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> MarkCircleGoodAsync([FromBody] MarkGoodPostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 把帖子移除圈子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("moveoutfromcircle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> MoveOutFromCircleAsync([FromBody] MovePostOutFromCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 我赞过的帖子
        /// </summary>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("likes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetLikedPostsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var posts = await _postQueries.GetLikedPostsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
        }

        /// <summary>
        /// 具有某个标签的帖子
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("tags/public/{tag}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetPostsByPublicTagAsync(string tag, [FromQuery] PagingParameters pagingParameters)
        {
            var posts = await _postQueries.GetPostsByPublicTagAsync(tag, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
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
        public async Task<ActionResult<bool>> ShareAsync([FromBody] ShareCommand sharePostCommand)
        {
            var result = await _mediator.Send(sharePostCommand);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取分享的帖子的详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("share/{encryptedPostId}/{encryptedSharedUserId}/{timestamp}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PostViewModel>> GetSharedPostAsync(string encryptedPostId, string encryptedSharedUserId, string encryptedTimestamp)
        {
            var postId = DecryptGuid(encryptedPostId);
            var sharedUserId = DecryptGuid(encryptedSharedUserId);
            var timestamp = DecryptTimestamp(encryptedTimestamp);

            _logger.LogInformation("postId: {postId}, sharedUserId: {sharedUserId}, timestamp: {timestamp}", postId, sharedUserId, timestamp);

            if (postId != Guid.Empty && sharedUserId != Guid.Empty && CheckShareTime(timestamp))
            {
                var post = await _postQueries.GetSharedPostAsync(postId, sharedUserId);
                return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
            }

            return new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper((StatusCode)HttpStatusCode.BadRequest, "分享的帖子不存在或已过期"));
        }

        /// <summary>
        /// 获取分享的类别下的所有帖子
        /// </summary>
        /// <param name="encryptedPrivateTag"></param>
        /// <param name="encryptedSharedUserId"></param>
        /// <param name="encryptedTimestamp"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("share/privatetag/{privateTag}/{encryptedSharedUserId}/{timestamp}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PostViewModel>> GetSharedPostAsync(string encryptedPrivateTag, string encryptedSharedUserId, string encryptedTimestamp, [FromQuery] PagingParameters pagingParameters)
        {
            var privateTag = Encryptor.DecryptDES(encryptedPrivateTag, _decryptKey);
            var sharedUserId = DecryptGuid(encryptedSharedUserId);
            var timestamp = DecryptTimestamp(encryptedTimestamp);

            _logger.LogInformation("privateTag: {privateTag}, sharedUserId: {sharedUserId}, timestamp: {timestamp}", privateTag, sharedUserId, timestamp);

            if (sharedUserId != Guid.Empty && privateTag != encryptedPrivateTag && CheckShareTime(timestamp))
            {
                var post = await _postQueries.GetSharedPostsAsync(privateTag, sharedUserId, pagingParameters);
                return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
            }

            return new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper((StatusCode)HttpStatusCode.BadRequest, "分享的帖子不存在或已过期"));
        }

        /// <summary>
        /// 获取分享的用户的所有帖子
        /// </summary>
        /// <param name="encryptedSharedUserId"></param>
        /// <param name="encryptedTimestamp"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("share/{sharedUserId}/{timestamp}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PostViewModel>> GetSharedPostAsync(string encryptedSharedUserId, string encryptedTimestamp, [FromQuery] PagingParameters pagingParameters)
        {
            var sharedUserId = DecryptGuid(encryptedSharedUserId);
            var timestamp = DecryptTimestamp(encryptedTimestamp);

            _logger.LogInformation("sharedUserId: {sharedUserId}, timestamp: {timestamp}", sharedUserId, timestamp);

            if (sharedUserId != Guid.Empty && CheckShareTime(timestamp))
            {
                var post = await _postQueries.GetSharedPostsAsync(sharedUserId, pagingParameters);
                return Ok(ResponseWrapper.CreateOkResponseWrapper(post));
            }

            return new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper((StatusCode)HttpStatusCode.BadRequest, "分享的帖子不存在或已过期"));
        }

        private bool CheckShareTime(double createdSeconds)
        {
            var validSeconds = _configuration.GetValue<int>("ShareValidTime") * 3600;
            var curSeconds = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return createdSeconds + validSeconds >= curSeconds;
        }

        private Guid DecryptGuid(string encryptedGuid)
        {
            try
            {
                return Guid.Parse(Encryptor.DecryptDES(encryptedGuid, _decryptKey));
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private double DecryptTimestamp(string encryptedTimestamp)
        {
            try
            {
                var timestamp = Encryptor.DecryptDES(encryptedTimestamp, _decryptKey);
                if (timestamp != encryptedTimestamp)
                {
                    double seconds = 0;
                    if (double.TryParse(timestamp, out seconds))
                        return seconds;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        [HttpGet]
        [Route("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public void test()
        {
            //var text = "630f1fa9-f11a-43ff-9145-5ec72d552ad7";
            //var encrypt = Encryptor.EncryptDES(text, _decryptKey);
            //var descypt = Encryptor.DecryptDES(encrypt, _decryptKey);
            //_logger.LogInformation("***********原文：{text}, 密文：{encrypt}，解密后：{descypt}", text, encrypt, descypt);

            _logger.LogInformation(Encryptor.DecryptDES("jPDW4S/qg4pADmkz+uaAKNecOi0STm3dWIAaUqsvFbiB5i9ak3qoWA==", _decryptKey));
            _logger.LogInformation(Encryptor.DecryptDES("YYksKFNKcoyb+6YXiUR0IeDSuTal9Xi0eAdsREk07wbEMIHhNlFJzQ==", _decryptKey));
        }
    }
}
