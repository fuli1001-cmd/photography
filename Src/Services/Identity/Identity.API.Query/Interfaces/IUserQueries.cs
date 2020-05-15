using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.API.Query.Interfaces
{
    public interface IUserQueries
    {
        UserViewModel GetCurrentUserAsync();
        List<FriendViewModel> GetFriendsAsync();
        IQueryable<Guid> GetFriendsIds(string userId);
    }
}
