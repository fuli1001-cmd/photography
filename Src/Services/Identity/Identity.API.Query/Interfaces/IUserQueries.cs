using Photography.Services.Identity.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Identity.API.Query.Interfaces
{
    public interface IUserQueries
    {
        UserViewModel GetCurrentUserAsync();
        List<FriendViewModel> GetFriendsAsync();
    }
}
