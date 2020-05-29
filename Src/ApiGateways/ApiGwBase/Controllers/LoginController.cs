using Arise.DDD.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Services;
using Photography.ApiGateways.ApiGwBase.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class LoginController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AuthService authService,
            UserService userService,
            ILogger<LoginController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 电话登录
        /// </summary>
        /// <param name="loginPhoneDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("phone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokensViewModel>> LoginWithPhoneAsync([FromBody] LoginPhoneDto loginPhoneDto)
        {
            var code = "Code" + loginPhoneDto.Code;
            
            if (await _authService.LoginWithPhoneNumberAsync(loginPhoneDto.PhoneNumber, code))
            {
                var result = await _userService.LoginWithPhoneNumberAsync(loginPhoneDto.PhoneNumber, code, loginPhoneDto.ClientType, loginPhoneDto.RegistrationId);
                await _authService.ChangeToRandomPasswordAsync(loginPhoneDto.PhoneNumber, code, Path.GetRandomFileName().Replace(".", string.Empty));
                return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
            }

            return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, "登录失败。"));
        }
    }
}
