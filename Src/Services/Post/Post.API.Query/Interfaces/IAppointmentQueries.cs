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
        Task<List<AppointmentViewModel>> GetAppointmentsAsync(PayerType? payerType, double? appointmentSeconds);

        // 获取我发起的约拍列表
        Task<List<AppointmentViewModel>> GetSentAppointmentsAsync();

        // 获取我收到的约拍列表
        Task<List<AppointmentViewModel>> GetReceivedAppointmentsAsync();
    }
}
