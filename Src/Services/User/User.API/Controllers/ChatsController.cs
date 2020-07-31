using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.BackwardCompatibility.Utils;
using Photography.Services.User.API.Query.BackwardCompatibility;
using Photography.Services.User.API.Query.Interfaces.BackwardCompatibility;
using Photography.Services.User.Domain.BackwardCompatibility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatQueries _chatQueries;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(IChatQueries chatQueries, ILogger<ChatsController> logger)
        {
            _chatQueries = chatQueries ?? throw new ArgumentNullException(nameof(chatQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取离线和近期消息
        /// </summary>
        /// <param name="latestMsgId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("offline_recent_messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChatMessage>> GetOfflineAndRecentMessagesAsync([FromQuery] string latestMsgId)
        {
            var messages = await _chatQueries.GetOfflineAndRecentMessagesAsync(latestMsgId);

            var encryptKey = "Ars!1&90";

            foreach (var m in messages.OfflineMsgs)
                m.content = Encryptor.DecryptDES(m.content, encryptKey);

            foreach (var m in messages.RecentMsgs)
                m.content = Encryptor.DecryptDES(m.content, encryptKey);

            return Ok(ResponseWrapper.CreateOkResponseWrapper(messages));
        }

        /// <summary>
        /// 获取聊天错误码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ErrorCodes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<List<PSR_ARS_ErrorCode>>> GetErrorCodesAsync()
        {
            var result = await _chatQueries.GetErrorCodesAsync();

            return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
        }
    }
}
