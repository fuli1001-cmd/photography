using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Tag.CreatePrivateTag;
using Photography.Services.Post.API.Application.Commands.Tag.DeletePrivateTag;
using Photography.Services.Post.API.Query.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly ITagQueries _tagQueries;
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator, ITagQueries tagQueries, ILogger<TagsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tagQueries = tagQueries ?? throw new ArgumentNullException(nameof(tagQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 系统标签
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("system")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper>> GetSystemTagsAsync()
        {
            var tags = await _tagQueries.GetSystemTagsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(tags));
        }

        /// <summary>
        /// 推荐标签
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("public/popular")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> GetPopularPublicTagsAsync()
        {
            var tags = await _tagQueries.GetPopularPublicTagsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(tags));
        }

        /// <summary>
        /// 用户的帖子类别
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper>> GetUserPrivateTagsAsync(Guid userId)
        {
            var tags = await _tagQueries.GetUserPrivateTagsAsync(userId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(tags));
        }

        /// <summary>
        /// 创建帖子类别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> CreatePrivateTagAsync([FromBody] CreatePrivateTagCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 删除帖子类别
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("private")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> DeletePrivateTagAsync([FromBody] DeletePrivateTagCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
