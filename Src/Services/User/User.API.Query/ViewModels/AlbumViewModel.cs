using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class AlbumViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string CoverPhoto { get; set; }

        public int PhotoCount { get; set; }
    }
}
