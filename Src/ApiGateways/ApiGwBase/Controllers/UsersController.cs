//using Arise.DDD.API.Response;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Photography.ApiGateways.ApiGwBase.Services;
//using Photography.ApiGateways.ApiGwBase.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Photography.ApiGateways.ApiGwBase.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [ApiVersion("1.0")]
//    public class UsersController : ControllerBase
//    {
//        private readonly NotificationService _notificationService;
//        private readonly UserService _userService;
//        private readonly ILogger<UsersController> _logger;

//        public UsersController(NotificationService notificationService,
//            UserService userService, 
//            ILogger<UsersController> logger)
//        {
//            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
//            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        /// <summary>
//        /// 获取当前用户信息
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("me")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<ActionResult<UserDto>> GetCurrentUserAsync([FromHeader]string authorization)
//        {
//            var userDtoTask = _userService.GetUserInfoAsync(authorization);
//            var pushSettingsTask = _notificationService.GetPushSettingsAsync(authorization);

//            await Task.WhenAll(new Task[] { userDtoTask, pushSettingsTask });

//            var userDto = userDtoTask.Result;
//            var pushSettings = pushSettingsTask.Result;

//            userDto.PushFollowEvent = pushSettings.PushFollowEvent;
//            userDto.PushForwardEvent = pushSettings.PushForwardEvent;
//            userDto.PushLikeEvent = pushSettings.PushLikeEvent;
//            userDto.PushReplyEvent = pushSettings.PushReplyEvent;
//            userDto.PushShareEvent = pushSettings.PushShareEvent;

//            return Ok(ResponseWrapper.CreateOkResponseWrapper(userDto));
//        }
//    }
//}
