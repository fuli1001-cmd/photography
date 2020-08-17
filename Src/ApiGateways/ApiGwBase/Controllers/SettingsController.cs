using Arise.DDD.API;
using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.ApiGateways.ApiGwBase.Services;
using Photography.ApiGateways.ApiGwBase.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Controllers
{
    /// <summary>
    /// 设置控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class SettingsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly ILogger<SettingsController> _logger;
        private readonly IOptionsSnapshot<ServerSettings> _serverSettings;

        public SettingsController(PostService postService, 
            IOptionsSnapshot<ServerSettings> serverSettings, 
            ILogger<SettingsController> logger)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ServerSettings>> GetServerSettingsAsync()
        {
            var settings = _serverSettings.Value;
            settings.SystemPostTags = await _postService.GetSystemTagsAsync();
            return Ok(ResponseWrapper.CreateOkResponseWrapper(settings));
        }
    }
}
