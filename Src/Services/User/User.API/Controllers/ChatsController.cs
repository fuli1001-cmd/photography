using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.BackwardCompatibility.Utils;
using Photography.Services.User.API.Query.BackwardCompatibility;
using Photography.Services.User.API.Query.Interfaces.BackwardCompatibility;
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
        [Route("offline_recent_messages/{latestMsgId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChatMessage>> GetOfflineAndRecentMessagesAsync(string latestMsgId)
        {
            var messages = await _chatQueries.GetOfflineAndRecentMessagesAsync(latestMsgId);

            var encryptKey = "Ars!1&90";

            foreach (var m in messages.OfflineMsgs)
                m.content = Encryptor.DecryptDES(m.content, encryptKey);

            foreach (var m in messages.RecentMsgs)
                m.content = Encryptor.DecryptDES(m.content, encryptKey);

            return Ok(ResponseWrapper.CreateOkResponseWrapper(messages));
        }
    }
}
