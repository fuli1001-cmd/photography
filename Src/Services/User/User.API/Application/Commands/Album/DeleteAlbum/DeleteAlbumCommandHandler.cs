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

namespace Photography.Services.User.API.Application.Commands.Album.DeleteAlbum
{
    public class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, bool>
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteAlbumCommandHandler> _logger;

        public DeleteAlbumCommandHandler(
            IAlbumRepository albumRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteAlbumCommandHandler> logger)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
        {
            var album = await _albumRepository.GetByIdAsync(request.AlbumId);

            if (album == null)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (album.UserId != myId)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not belong to user {myId}." });

            album.Delete();

            _albumRepository.Remove(album);

            return await _albumRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
