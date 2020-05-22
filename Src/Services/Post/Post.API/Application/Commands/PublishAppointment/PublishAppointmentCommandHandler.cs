using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.PublishAppointment
{
    public class PublishAppointmentCommandHandler : IRequestHandler<PublishAppointmentCommand, AppointmentViewModel>
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PublishAppointmentCommandHandler> _logger;

        public PublishAppointmentCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ILogger<PublishAppointmentCommandHandler> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentViewModel> Handle(PublishAppointmentCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var attachments = request.Attachments.Select(a => new PostAttachment(a.Name, a.Text, a.AttachmentType)).ToList();
            var post = new Domain.AggregatesModel.PostAggregate.Post(request.Text, request.AppointedTime, request.Price, request.PayerType,
                request.Latitude, request.Longitude, request.LocationName, request.CityCode, attachments, Guid.Parse(userId));
            _postRepository.Add(post);
            await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            _postRepository.LoadUser(post);
            return _mapper.Map<AppointmentViewModel>(post);
        }
    }
}
