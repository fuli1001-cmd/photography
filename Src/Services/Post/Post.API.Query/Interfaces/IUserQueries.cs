using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IUserQueries
    {
        UserViewModel GetCurrentUserAsync();
        List<FriendViewModel> GetFriendsAsync();
        IQueryable<Guid> GetFriendsIds(string userId);
    }
}
