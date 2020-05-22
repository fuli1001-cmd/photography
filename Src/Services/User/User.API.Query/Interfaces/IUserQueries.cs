using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces
{
    public interface IUserQueries
    {
        UserViewModel GetCurrentUserAsync();
        List<FriendViewModel> GetFriendsAsync();
    }
}
