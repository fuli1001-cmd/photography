using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.UpdateUserShare
{
    public class UpdateUserShareCommandHandler : IRequestHandler<UpdateUserShareCommand, bool>
    {
        private readonly IUserShareRepository _userShareRepository;
        private readonly ILogger<UpdateUserShareCommandHandler> _logger;

        public UpdateUserShareCommandHandler(IUserShareRepository userShareRepository,
            ILogger<UpdateUserShareCommandHandler> logger)
        {
            _userShareRepository = userShareRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserShareCommand request, CancellationToken cancellationToken)
        {
            UserShare userShare = null;
            
            if (request.PostId != null)
                userShare = await _userShareRepository.GetUserShareAsync(request.SharerId, request.PostId.Value);
            else if (!string.IsNullOrWhiteSpace(request.PrivateTag))
                userShare = await _userShareRepository.GetUserShareAsync(request.SharerId, request.PrivateTag);
            else
                userShare = await _userShareRepository.GetUserShareAsync(request.SharerId);

            if (userShare == null)
            {
                if (request.PostId != null)
                    userShare = new UserShare(request.SharerId, request.PostId.Value);
                else if (!string.IsNullOrWhiteSpace(request.PrivateTag))
                    userShare = new UserShare(request.SharerId, request.PrivateTag);
                else
                    userShare = new UserShare(request.SharerId);

                _userShareRepository.Add(userShare);
            }

            userShare.IncreaseVisitCount();

            return await _userShareRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
