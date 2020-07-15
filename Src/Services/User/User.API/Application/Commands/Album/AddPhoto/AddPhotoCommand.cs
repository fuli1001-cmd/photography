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
        /// 要加入的照片名称和显示名称数组
        /// </summary>
        public List<PhotoNameInfo> Names { get; set; }
    }

    public class PhotoNameInfo
    {
        /// <summary>
        /// 照片文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 照片文件显示名
        /// </summary>
        public string DisplayName { get; set; }
    }
}
