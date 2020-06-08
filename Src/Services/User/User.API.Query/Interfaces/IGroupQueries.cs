using Arise.DDD.API.Paging;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces
{
    public interface IGroupQueries
    {
        Task<GroupViewModel> GetGroupAsync(Guid groupId);
        Task<PagedList<GroupViewModel>> GetGroupsAsync(PagingParameters pagingParameters);
    }
}
