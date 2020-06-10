using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IAppointmentQueries
    {
        // 获取约拍广场的约拍列表
        Task<PagedList<AppointmentViewModel>> GetAppointmentsAsync(PayerType? payerType, double? appointmentSeconds, PagingParameters pagingParameters);

        // 获取我发布的约拍
        Task<PagedList<AppointmentViewModel>> GetMyAppointmentsAsync(PagingParameters pagingParameters);

        // 获取约拍详情
        Task<AppointmentViewModel> GetAppointmentAsync(Guid appointmentId);
    }
}
