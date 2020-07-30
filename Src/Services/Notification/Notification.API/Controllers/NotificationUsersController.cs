using Arise.DDD.API.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.Notification.API.Application.Commands.ConfigurePush;
using Photography.Services.Notification.API.Application.Commands.ConfigurePushByEventType;
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
    public class NotificationUsersController : ControllerBase
    {
        private readonly ILogger<NotificationUsersController> _logger;
        private readonly IUserQueries _userQueries;
        private readonly IMediator _mediator;

        public NotificationUsersController(IMediator mediator, IUserQueries userQueries, ILogger<NotificationUsersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 推送设置v1
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("pushsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete]
        public async Task<ActionResult<bool>> ConfigurePushAsync([FromBody] ConfigurePushByEventTypeCommand command)
        {
            _logger.LogInformation("old api called: pushsettings");

            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 推送设置v2
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("pushsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ApiVersion("2.0")]
        public async Task<ActionResult<bool>> ConfigurePushAsync([FromBody] ConfigurePushCommand command)
        {
            _logger.LogInformation("pushsettings v2");

            var result = await _mediator.Send(command);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }

        /// <summary>
        /// 获取推送设置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("pushsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PushSettingsViewModel>> GetCurrentUserAsync()
        {
            var pushSettings = await _userQueries.GetPushSettingsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(pushSettings));
        }
    }
}
