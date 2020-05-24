using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointTask
{
    public class AppointTaskCommandHandler : IRequestHandler<AppointTaskCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointTaskCommandHandler> _logger;

        public AppointTaskCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ILogger<AppointTaskCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AppointTaskCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();
            var appointment = await _postRepository.GetByIdAsync(request.AppointmentId);
            
            PayerType payerType = PayerType.Free;
            if (appointment.PayerType == PayerType.Me)
                payerType = PayerType.You;
            else if (appointment.PayerType == PayerType.You)
                payerType = PayerType.Me;

            var post = Domain.AggregatesModel.PostAggregate.Post.CreateAppointmentDeal(
                request.Text, appointment.AppointedTime, appointment.Price, payerType, 
                appointment.Latitude, appointment.Longitude, appointment.LocationName, appointment.Address, 
                appointment.CityCode, attachments, Guid.Parse(userId), appointment.UserId, appointment.Id);
            _postRepository.Add(post);

            return await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
