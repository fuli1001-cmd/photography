using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.Interfaces
{
    public interface IUserQueries
    {
        UserViewModel GetCurrentUserAsync();
        List<FriendViewModel> GetFriendsAsync();
    }
}
