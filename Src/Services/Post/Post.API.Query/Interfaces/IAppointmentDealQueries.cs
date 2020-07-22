using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IAppointmentDealQueries
    {
        // 获取我发出的约拍交易列表
        Task<PagedList<AppointmentViewModel>> GetSentAppointmentDealsAsync(PagingParameters pagingParameters);

        // 获取我收到的约拍交易列表
        Task<PagedList<AppointmentViewModel>> GetReceivedAppointmentDealsAsync(PagingParameters pagingParameters);

        Task<AppointmentViewModel> GetSentAppointmentDealAsync(Guid dealId);
    }
}
