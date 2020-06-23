using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.UpdatePhoto
{
    public class UpdatePhotoCommandHandler : IRequestHandler<UpdatePhotoCommand, bool>
    {
        private readonly IAlbumPhotoRepository _albumPhotoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdatePhotoCommandHandler> _logger;

        public UpdatePhotoCommandHandler(
            IAlbumPhotoRepository albumPhotoRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdatePhotoCommandHandler> logger)
        {
            _albumPhotoRepository = albumPhotoRepository ?? throw new ArgumentNullException(nameof(albumPhotoRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
        {
            var photoIds = request.UpdatePhotoInfos.Select(x => x.PhotoId);
            var photos = await _albumPhotoRepository.GetPhotosAsync(photoIds);

            photos.ForEach(p =>
            {
                p.UpdateDisplayName(request.UpdatePhotoInfos.Where(x => x.PhotoId == p.Id).Select(x => x.DisplayName).SingleOrDefault());
            });

            return await _albumPhotoRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
