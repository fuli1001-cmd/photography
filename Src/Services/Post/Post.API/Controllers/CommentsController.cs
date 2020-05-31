﻿using Arise.DDD.API;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Comment.ReplyComment;
using Photography.Services.Post.API.Application.Commands.Comment.ReplyPost;
using Photography.Services.Post.API.Application.Commands.Comment.ToggleLikeComment;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly ICommentQueries _commentQueries;
        private readonly IMediator _mediator;

        public CommentsController(IMediator mediator, ICommentQueries commentQueries, ILogger<CommentsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _commentQueries = commentQueries ?? throw new ArgumentNullException(nameof(commentQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取某个帖子的评论
        /// </summary>
        /// <param name="postId">帖子id</param>
        /// <param name="maxSubCommentsCount">最多返回的子评论数量</param>
        /// <returns></returns>
        [HttpGet]
        [Route("post/{postId}/{maxSubCommentsCount}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetPostCommentsAsync(Guid postId, int maxSubCommentsCount)
        {
            var comments = await _commentQueries.GetPostCommentsAsync(postId, maxSubCommentsCount);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(comments));
        }

        /// <summary>
        /// 获取某个评论的评论
        /// </summary>
        /// <param name="commentId">评论id</param>
        /// <param name="maxSubCommentsCount">最多返回的子评论数量</param>
        /// <returns></returns>
        [HttpGet]
        [Route("comment/{commentId}/{maxSubCommentsCount}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetCommentCommentsAsync(Guid commentId, int maxSubCommentsCount)
        {
            var comments = await _commentQueries.GetSubCommentsAsync(commentId, maxSubCommentsCount);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(comments));
        }

        /// <summary>
        /// 回复帖子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ReplyPostAsync([FromBody] ReplyPostCommand command)
        {
            var posts = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        /// <summary>
        /// 回复评论
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ReplyCommentAsync([FromBody] ReplyCommentCommand command)
        {
            var posts = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(posts));
        }

        [HttpPut]
        [Route("togglelike")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ToggleLikeCommentAsync([FromBody] ToggleLikeCommentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
