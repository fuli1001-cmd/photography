using ApplicationMessages.Events;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.IntegrationEventHandlers
{
    public class PostLikedEventHandler : IHandleMessages<PostLikedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostLikedEventHandler> _logger;

        public PostLikedEventHandler(IUserRepository userRepository, ILogger<PostLikedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PostLikedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling PostLikedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var postUser = await _userRepository.GetByIdAsync(message.PostUserId);
                postUser.IncreaseLikedCount();

                // 点赞帖子和点赞评论时都会发送本事件，但是点赞评论不需要增加点赞人的点赞帖子数，
                // 因此点赞评论时没有传点赞人的id过来，当点赞人（LikingUserId）id为空时，表示不需要增加点赞人的点赞帖子数量
                if (message.LikingUserId != Guid.Empty)
                {
                    var likingUser = await _userRepository.GetByIdAsync(message.LikingUserId);
                    likingUser.IncreaseLikedPostCount();
                }

                await _userRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
