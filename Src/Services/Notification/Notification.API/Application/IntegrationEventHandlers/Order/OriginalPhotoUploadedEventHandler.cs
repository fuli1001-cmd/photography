﻿using ApplicationMessages.Events.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Services.Notification.API.Application.Commands.CreateEvent;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Application.IntegrationEventHandlers.Order
{
    public class OriginalPhotoUploadedEventHandler : IHandleMessages<OriginalPhotoUploadedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<OriginalPhotoUploadedEventHandler> _logger;

        public OriginalPhotoUploadedEventHandler(IUserRepository userRepository, IMediator mediator, ILogger<OriginalPhotoUploadedEventHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OriginalPhotoUploadedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling OriginalPhotoUploadedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                var nickName = await _userRepository.GetNickNameAsync(message.UploadPhotoUserId);

                // 创建原片已上传的事件
                var command = new CreateEventCommand
                {
                    FromUserId = message.UploadPhotoUserId,
                    ToUserId = message.AnotherUserId,
                    EventType = Domain.AggregatesModel.EventAggregate.EventType.OriginalPhotoUploaded,
                    CommentText = "已上传原片",
                    OrderId = message.OrderId,
                    PushMessage = $"{nickName}已上传原片"
                };

                await _mediator.Send(command);
            }
        }
    }
}
