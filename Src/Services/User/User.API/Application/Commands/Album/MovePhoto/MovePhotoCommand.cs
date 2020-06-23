using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.MovePhoto
{
    public class MovePhotoCommand : IRequest<bool>
    {
        /// <summary>
        /// 要移动到的相册id
        /// </summary>
        public Guid NewAlbumId { get; set; }

        /// <summary>
        /// 要移动的照片
        /// </summary>
        public List<Guid> PhotoIds { get; set; }
    }
}
