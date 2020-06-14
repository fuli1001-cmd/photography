using Aliyun.Acs.Core.Exceptions;
using Arise.DDD.API;
using Arise.DDD.API.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photography.ApiGateways.ApiGwBase.Dtos;
using Photography.ApiGateways.ApiGwBase.Redis;
using Photography.ApiGateways.ApiGwBase.Services;
using Photography.ApiGateways.ApiGwBase.Sms;
using System;
using System.IO;
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
        private readonly ISmsService _smsService;
        private readonly IRedisService _redisService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AuthService authService,
            UserService userService,
            ISmsService smsService,
            IRedisService redisService,
            ILogger<LoginController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="sendVerifyCodeDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> GetVerifyCode([FromBody] SendVerifyCodeDto sendVerifyCodeDto)
        {
            var code = _smsService.SendSms(sendVerifyCodeDto.Phonenumber);
            await _redisService.SetAsync(sendVerifyCodeDto.Phonenumber, code, TimeSpan.FromSeconds(300));
            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
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
            // verify code
            var storedCode = await _redisService.GetAsync(loginPhoneDto.PhoneNumber);
            if (string.IsNullOrEmpty(storedCode) || storedCode.ToLower() != loginPhoneDto.Code.ToLower())
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((StatusCode)(int)HttpStatusCode.BadRequest, "验证码错误。"));

            var code = "Code" + loginPhoneDto.Code;
            
            if (await _authService.LoginWithPhoneNumberAsync(loginPhoneDto.PhoneNumber, code))
            {
                var result = await _userService.LoginWithPhoneNumberAsync(loginPhoneDto.PhoneNumber, code, loginPhoneDto.ClientType, loginPhoneDto.RegistrationId);
                await _authService.ChangeToRandomPasswordAsync(loginPhoneDto.PhoneNumber, code, Path.GetRandomFileName().Replace(".", string.Empty));
                return Ok(ResponseWrapper.CreateOkResponseWrapper(result));
            }

            // internal server error occured
            throw new ApplicationException("登录失败。");
        }
    }
}
