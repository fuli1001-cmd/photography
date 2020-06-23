using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.UpdatePhoto
{
    public class UpdatePhotoCommand : IRequest<bool>
    {
        public List<UpdatePhotoInfo> UpdatePhotoInfos { get; set; }
    }

    public class UpdatePhotoInfo
    {
        public Guid PhotoId { get; set; }

        public string DisplayName { get; set; }
    }
}
