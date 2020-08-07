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
using Newtonsoft.Json;
using Photography.Services.Post.API.Application.Commands.User.UpdateUserShare;
using Photography.Services.Post.API.Application.Commands.Post.ExaminePost;
using Photography.Services.Post.API.Application.Commands.Post.ViewPost;
using Photography.Services.Post.API.Application.Commands.Post.ToggleUserLikePost;

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
        /// 获取用户的帖子数量，约拍数量，点赞的帖子数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("PostAndAppointmentCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PostCountViewModel>> GetUserPostAndAppointmentCountAsync()
        {
            var posts = await _postQueries.GetUserPostAndAppointmentCountAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
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
        /// 此接口用于查看帖子详情时增加帖子积分
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("view/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PostViewModel>> ViewPostAsync(Guid postId)
        {
            var command = new ViewPostCommand { PostId = postId };
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
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
        /// 获取分享的帖子
        /// </summary>
        /// <param name="s">加密过的分享信息</param>
        /// <param name="t">帖子类别，当分享的是用户的所有帖子时，可根据类别来筛选</param>
        /// <param name="k">搜索关键字</param>
        /// <param name="c">是否统计访问次数</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("share")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper>> GetShareDataAsync([FromQuery]string s, [FromQuery]string t, [FromQuery]string k, [FromQuery] bool c, [FromQuery] PagingParameters pagingParameters)
        {
            PagedList<PostViewModel> posts = null;

            var descryptedStr = Encryptor.DecryptDES(s, _decryptKey, _decryptKey);

            if (descryptedStr != s)
            {
                var shareInfo = JsonConvert.DeserializeObject<ShareInfo>(descryptedStr);

                _logger.LogInformation("GetShareDataAsync: {@ShareInfo}", shareInfo);

                // 优先搜索加密对象中的分类，若其无值，则搜索url参数中的分类
                // 其场景是：客户端可以分享三种类型，1. 单个帖子，2. 帖子分类，3. 用户的全部帖子
                // 当分享的是第三种时，加密对象中只有用户id（帖子id和帖子分类为空）
                // 在网页上查看分享时，对与第三种可以选择帖子分类来筛选，选择的分类通过url参数privateTag传上来
                if (string.IsNullOrWhiteSpace(shareInfo.PrivateTag))
                    shareInfo.PrivateTag = t;

                if (shareInfo.UserId != Guid.Empty && CheckShareTime(shareInfo.Timestamp))
                {
                    if (shareInfo.PostId != Guid.Empty)
                    {
                        _logger.LogInformation("GetShareDataAsync: single post");

                        // 统计访问次数
                        if (c)
                            await UpdateUserShareAsync(shareInfo.UserId, shareInfo.PostId, null);

                        // get share data and return
                        var post = await _postQueries.GetSharedPostAsync(shareInfo.PostId, shareInfo.UserId);
                        posts = new PagedList<PostViewModel>(new List<PostViewModel> { post }, 1, new PagingParameters { PageNumber = 1, PageSize = 1 });
                        return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
                    }
                    else if (!string.IsNullOrWhiteSpace(shareInfo.PrivateTag))
                    {
                        _logger.LogInformation("GetShareDataAsync: tag posts");

                        // 统计访问次数
                        if (c)
                            await UpdateUserShareAsync(shareInfo.UserId, null, shareInfo.PrivateTag);

                        // get share data and return
                        posts = await _postQueries.GetSharedPostsAsync(shareInfo.PrivateTag, shareInfo.UserId, k, pagingParameters);
                        return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
                    }
                    else
                    {
                        _logger.LogInformation("GetShareDataAsync: user posts");

                        // 统计访问次数
                        if (c)
                            await UpdateUserShareAsync(shareInfo.UserId, null, null);

                        // get share data and return
                        posts = await _postQueries.GetSharedPostsAsync(shareInfo.UserId, k, pagingParameters);
                        return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(posts));
                    }
                }
            }

            return new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper((StatusCode)HttpStatusCode.BadRequest, "分享的帖子不存在或已过期"));
        }

        private async Task UpdateUserShareAsync(Guid userId, Guid? postId, string privateTag)
        {
            var command = new UpdateUserShareCommand { SharerId = userId, PostId = postId, PrivateTag = privateTag };
            await _mediator.Send(command);
        }

        private bool CheckShareTime(double createdSeconds)
        {
            var validSeconds = _configuration.GetValue<int>("ShareValidHours") * 3600;
            var curSeconds = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return createdSeconds + validSeconds >= curSeconds;
        }

        #region AdminOnly
        /// <summary>
        /// 审核帖子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("examine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<bool>> ExamineAsync([FromBody] ExaminePostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 指定用户赞一个帖子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("toggleuserlike")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<bool>> UserLikePostAsync([FromBody] ToggleUserLikePostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
        #endregion
    }

    public class ShareInfo
    {
        public Guid PostId { get; set; }
        public string PrivateTag { get; set; }
        public Guid UserId { get; set; }
        public double Timestamp { get; set; }
        public bool NoAd { get; set; }
    }
}
