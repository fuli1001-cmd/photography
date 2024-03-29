﻿using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Application.Commands.Circle.AddCircleMember;
using Photography.Services.Post.API.Application.Commands.Circle.ChangeCircleOwner;
using Photography.Services.Post.API.Application.Commands.Circle.CreateCircle;
using Photography.Services.Post.API.Application.Commands.Circle.DeleteCircle;
using Photography.Services.Post.API.Application.Commands.Circle.JoinCircle;
using Photography.Services.Post.API.Application.Commands.Circle.QuitCircle;
using Photography.Services.Post.API.Application.Commands.Circle.ToppingCircle;
using Photography.Services.Post.API.Application.Commands.Circle.UpdateCircle;
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
    public class CirclesController : ControllerBase
    {
        private readonly ILogger<CirclesController> _logger;
        private readonly ICircleQueries _circleQueries;
        private readonly IMediator _mediator;

        public CirclesController(IMediator mediator, ICircleQueries circleQueries, ILogger<CirclesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _circleQueries = circleQueries ?? throw new ArgumentNullException(nameof(circleQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 创建圈子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> CreateCircleAsync([FromBody] CreateCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 编辑圈子
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> UpdateCircleAsync([FromBody] UpdateCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 删除圈子
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> DeleteCircleAsync([FromBody] DeleteCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 圈子详情
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{circleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper>> GetCircleAsync(Guid circleId)
        {
            var result = await _circleQueries.GetCircleAsync(circleId);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 分页获取圈子列表，按圈子内人数倒序排列
        /// </summary>
        /// <param name="key">搜索关键字</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponseWrapper>> GetCirclesAsync([FromQuery(Name = "key")] string key, [FromQuery] PagingParameters pagingParameters)
        {
            var result = await _circleQueries.GetCirclesAsync(key, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(result));
        }

        /// <summary>
        /// 分页获取我的圈子列表，按置顶、入圈时间倒序排列
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetMyCirclesAsync([FromQuery] PagingParameters pagingParameters)
        {
            var result = await _circleQueries.GetMyCirclesAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(result));
        }

        /// <summary>
        /// 置顶或取消置顶圈子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("topping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> ToppingCircleAsync([FromBody] ToppingCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 用户申请或直接加入圈子（取决于圈子是否需要入圈审核）
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> JoinCircleAsync([FromBody] JoinCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 用户退出圈子
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("quit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> QuitCircleAsync([FromBody] QuitCircleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 圈子管理员加用户入圈
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adduser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> AddCircleMemberAsync([FromBody] AddCircleMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 转让圈子
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("setowner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseWrapper>> UpdateCircleAsync([FromBody] ChangeCircleOwnerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
