using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.UpdateAlbum
{
    public class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand, bool>
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateAlbumCommandHandler> _logger;

        public UpdateAlbumCommandHandler(
            IAlbumRepository albumRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateAlbumCommandHandler> logger)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
        {
            var album = await _albumRepository.GetByIdAsync(request.AlbumId);

            if (album == null)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (album.UserId != myId)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not belong to user {myId}." });

            album.Update(request.NewName);

            return await _albumRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
