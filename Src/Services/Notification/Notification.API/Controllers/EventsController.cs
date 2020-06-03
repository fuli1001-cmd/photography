using Arise.DDD.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Query.Interfaces;
using Photography.Services.Notification.API.Query.ViewModels;
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

        public EventsController(IEventQueries eventQueries, ILogger<EventsController> logger)
        {
            _eventQueries = eventQueries ?? throw new ArgumentNullException(nameof(eventQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 用户的事件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventViewModel>>> GetOrderAsync()
        {
            var user = await _eventQueries.GetUserReceivedEventsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user));
        }
    }
}
