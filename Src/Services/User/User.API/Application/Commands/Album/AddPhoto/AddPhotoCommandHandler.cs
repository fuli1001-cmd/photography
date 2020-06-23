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

namespace Photography.Services.User.API.Application.Commands.Album.AddPhoto
{
    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, bool>
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AddPhotoCommandHandler> _logger;

        public AddPhotoCommandHandler(
            IAlbumRepository albumRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AddPhotoCommandHandler> logger)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            var album = await _albumRepository.GetAlbumWithPhotosAsync(request.AlbumId);

            if (album == null)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not exist." });

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (album.UserId != myId)
                throw new ClientException("操作失败", new List<string> { $"Album {request.AlbumId} does not belong to user {myId}." });

            request.Names.ForEach(name =>
            {
                var photo = new AlbumPhoto(name);
                album.AddPhoto(photo);
            });

            return await _albumRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
