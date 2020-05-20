using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.API.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.BackwardCompatibility.ViewModels
{
    public class InfoViewModel
    {
        public UserViewModel UserViewModel { get; set; }
        public ServerSettings ServerSettings { get; set; }
    }
}
