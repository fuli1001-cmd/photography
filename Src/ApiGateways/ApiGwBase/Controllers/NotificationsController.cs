using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class NotificationsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            PostService postService,
            NotificationService notificationService,
            ILogger<NotificationsController> logger)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取未读事件数量
        /// 由两部分构成：
        /// 1. 通知服务中的未读通知数量
        /// 2. 帖子服务中的发出和收到的约拍任务数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unread-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UnReadEventCountDto>> GetUnReadEventCountAsync()
        {
            // 获取未读通知数量
            var unReadEventCountTask = _notificationService.GetUnReadEventCountAsync();
            // 获取发出和收到的约拍任务数量
            var sentAndReceivedAppointmentDealCountTask = _postService.GetSentAndReceivedAppointmentDealCountAsync();
            var tasks = new List<Task<UnReadEventCountDto>> { unReadEventCountTask, sentAndReceivedAppointmentDealCountTask };
            await Task.WhenAll(tasks);

            // 合并结果
            var result = unReadEventCountTask.Result;
            result.SentAppointmentDeal = sentAndReceivedAppointmentDealCountTask.Result.SentAppointmentDeal;
            result.ReceivedAppointmentDeal = sentAndReceivedAppointmentDealCountTask.Result.ReceivedAppointmentDeal;

            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
