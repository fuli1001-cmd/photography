using ApplicationMessages.Events.Order;
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
    //public class OriginalPhotoUploadedEventHandler : IHandleMessages<OriginalPhotoUploadedEvent>
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly ILogger<OriginalPhotoUploadedEventHandler> _logger;

    //    public OriginalPhotoUploadedEventHandler(IUserRepository userRepository, ILogger<OriginalPhotoUploadedEventHandler> logger)
    //    {
    //        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }

    //    public async Task Handle(OriginalPhotoUploadedEvent message, IMessageHandlerContext context)
    //    {
    //        using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
    //        {
    //            _logger.LogInformation("----- Handling OriginalPhotoUploadedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

    //            // 减少用户1的拍片阶段订单数量，并增加他的选片阶段订单数量
    //            var user1 = await _userRepository.GetByIdAsync(message.User1Id);
    //            user1.DecreaseShootingStageOrderCount();
    //            user1.IncreaseSelectionStageOrderCount();

    //            // 减少用户2的拍片阶段订单数量，并增加他的选片阶段订单数量
    //            var user2 = await _userRepository.GetByIdAsync(message.User2Id);
    //            user2.DecreaseShootingStageOrderCount();
    //            user2.IncreaseSelectionStageOrderCount();

    //            await _userRepository.UnitOfWork.SaveEntitiesAsync();
    //        }
    //    }
    //}
}
