using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.Post.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Controllers
{
    /// <summary>
    /// 设置控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IOptionsSnapshot<ServerSettings> _serverSettings;

        public SettingsController(IOptionsSnapshot<ServerSettings> serverSettings, ILogger<SettingsController> logger)
        {
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
        public ActionResult<ServerSettings> GetServerSettings()
        {
            return Ok(_serverSettings.Value);
        }
    }
}
