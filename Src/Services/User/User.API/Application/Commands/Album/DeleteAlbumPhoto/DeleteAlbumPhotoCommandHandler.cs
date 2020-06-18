using Arise.DDD.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.DeleteAlbumPhoto
{
    public class DeleteAlbumPhotoCommandHandler : IRequestHandler<DeleteAlbumPhotoCommand, bool>
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IAlbumPhotoRepository _albumPhotoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteAlbumPhotoCommandHandler> _logger;

        public DeleteAlbumPhotoCommandHandler(
            IAlbumRepository albumRepository,
            IAlbumPhotoRepository albumPhotoRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteAlbumPhotoCommandHandler> logger)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
            _albumPhotoRepository = albumPhotoRepository ?? throw new ArgumentNullException(nameof(albumPhotoRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteAlbumPhotoCommand request, CancellationToken cancellationToken)
        {
            var album = await _albumRepository.GetAlbumWithPhotosAsync(request.AlbumId);

            if (album == null)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (album.UserId != myId)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not belong to user {myId}." });

            foreach (var id in request.PhotoIds)
            {
                var photo = await _albumPhotoRepository.GetByIdAsync(id);
                if (photo.AlbumId == request.AlbumId)
                    _albumPhotoRepository.Remove(photo);
            }

            return await _albumPhotoRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
