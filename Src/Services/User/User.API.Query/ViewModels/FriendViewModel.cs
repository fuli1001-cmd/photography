using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class FriendViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public bool Muted { get; set; }
        public int ChatServerUserId { get; private set; }
    }
}
