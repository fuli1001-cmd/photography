using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Photography.Messages.Events;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AcceptAppointmentDeal
{
    public class AcceptAppointmentDealCommandHandler : IRequestHandler<AcceptAppointmentDealCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AcceptAppointmentDealCommandHandler> _logger;

        public AcceptAppointmentDealCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            ILogger<AcceptAppointmentDealCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AcceptAppointmentDealCommand request, CancellationToken cancellationToken)
        {
            var deal = await _postRepository.GetPostWithAppointmentedUserById(request.DealId);
            deal.AcceptAppointmentDeal(request.UserId);
            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
