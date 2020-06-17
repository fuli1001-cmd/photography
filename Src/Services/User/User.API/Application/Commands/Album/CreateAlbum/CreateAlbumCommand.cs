using MediatR;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Album.CreateAlbum
{
    public class CreateAlbumCommand : IRequest<AlbumViewModel>
    {
        public string Name { get; set; }
    }
}
