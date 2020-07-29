using Arise.DDD.API;
using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Application.Commands.ReadEvent;
using Photography.Services.Notification.API.Query.Interfaces;
using Photography.Services.Notification.API.Query.ViewModels;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IEventQueries _eventQueries;
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator, IEventQueries eventQueries, ILogger<EventsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventQueries = eventQueries ?? throw new ArgumentNullException(nameof(eventQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 用户的事件
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete]
        public async Task<ActionResult<PagedResponseWrapper>> GetUserReceivedEventsAsync([FromQuery] PagingParameters pagingParameters)
        {
            var result = await _eventQueries.GetUserReceivedEventsAsync(pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(result));
        }

        /// <summary>
        /// 按类别获取用户的事件
        /// </summary>
        /// <param name="eventCategory">事件类别</param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{eventCategory}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseWrapper>> GetUserCategoryEventsAsync(EventCategory eventCategory, [FromQuery] PagingParameters pagingParameters)
        {
            var result = await _eventQueries.GetUserCategoryEventsAsync(eventCategory, pagingParameters);
            return Ok(PagedResponseWrapper.CreateOkPagedResponseWrapper(result));
        }

        /// <summary>
        /// 获取未读事件数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unread-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UnReadEventCountViewModel>> GetUnReadEventCountAsync()
        {
            var result = await _eventQueries.GetUnReadEventCountAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 标记某类别的事件为已读
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("mark-event-category-readed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UnReadEventCountViewModel>> MarkEventCategoryReadedAsync([FromBody] ReadEventCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
