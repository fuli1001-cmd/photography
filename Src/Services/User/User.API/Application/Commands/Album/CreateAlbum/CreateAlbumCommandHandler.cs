using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Query.Interfaces;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.CreateAlbum
{
    public class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand, AlbumViewModel>
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IAlbumQueries _albumQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateAlbumCommandHandler> _logger;

        public CreateAlbumCommandHandler(
            IAlbumRepository albumRepository,
            IAlbumQueries albumQueries,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateAlbumCommandHandler> logger)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
            _albumQueries = albumQueries ?? throw new ArgumentNullException(nameof(albumQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AlbumViewModel> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var album = new Domain.AggregatesModel.AlbumAggregate.Album(request.Name, myId);
            _albumRepository.Add(album);
            
            if (await _albumRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
                return await _albumQueries.GetAlbumAsync(album.Id);

            throw new ApplicationException("操作失败");
        }
    }
}
