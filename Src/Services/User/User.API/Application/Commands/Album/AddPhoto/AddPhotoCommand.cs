using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.AddPhoto
{
    public class AddPhotoCommand : IRequest<bool>
    {
        /// <summary>
        /// 要加入到的相册id
        /// </summary>
        public Guid AlbumId { get; set; }

        /// <summary>
        /// 要加入的照片名称
        /// </summary>
        public List<string> Names { get; set; }
    }
}
