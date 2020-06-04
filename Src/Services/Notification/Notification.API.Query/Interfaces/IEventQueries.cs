using Arise.DDD.API.Paging;
using Photography.Services.Notification.API.Query.ViewModels;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Query.Interfaces
{
    public interface IEventQueries
    {
        Task<PagedList<EventViewModel>> GetUserReceivedEventsAsync(PagingParameters pagingParameters);
    }
}
