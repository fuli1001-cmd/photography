using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.DeleteAlbumPhoto
{
    public class DeleteAlbumPhotoCommand : IRequest<bool>
    {
        public Guid AlbumId { get; set; }
        public List<Guid> PhotoIds { get; set; }
    }
}
