using Arise.DDD.Infrastructure.Redis;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.Notification.API.Application.Commands.CreateEvent
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRedisService _redisService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CreateEventCommandHandler> _logger;

        public CreateEventCommandHandler(
            IEventRepository eventRepository, 
            IRedisService redisService, 
            IUserRepository userRepository,
            IWebHostEnvironment env,
            ILogger<CreateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // 不记录自己给自己产生的事件
            if (request.FromUserId != request.ToUserId)
            {
                //  创建通知
                var @event = new Event(request.FromUserId, request.ToUserId, request.EventType, request.PostId, 
                    request.CommentId, request.CommentText, request.CircleId, request.CircleName, request.OrderId);
                _eventRepository.Add(@event);
                var result = await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                // 推送
                _logger.LogInformation("push start push");
                await PushNotificationAsync(request.ToUserId, request.PushMessage);
            }
            return true;
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task PushNotificationAsync(Guid toUserId, string message)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(toUserId);

                _logger.LogInformation("push toUserId: " + toUserId);
                _logger.LogInformation("push user: {@user}", user);
                _logger.LogInformation("push message: " + message);

                if (user != null && !string.IsNullOrWhiteSpace(user.RegistrationId) && !string.IsNullOrWhiteSpace(message))
                {
                    var notification = new
                    {
                        DeviceId = user.RegistrationId,
                        Msg = message,
                        IsDebug = _env.IsDevelopment()
                    };

                    string json = SerializeUtil.SerializeToJson(notification);
                    var bytes = SerializeUtil.SerializeStringToBytes(json, true);
                    json = JsonConvert.SerializeObject(bytes);

                    await _redisService.StringSetAsync("PUBLISH_MSG", json, null);

                    _logger.LogInformation("PushNotification: {@PushNotification}", notification);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("push notification error: {@PushNotificationError}", ex);
            }
        }
    }
}
