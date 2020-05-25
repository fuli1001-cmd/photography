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
        Task<List<AppointmentViewModel>> GetSentAppointmentDealsAsync();

        // 获取我收到的约拍交易列表
        Task<List<AppointmentViewModel>> GetReceivedAppointmentDealsAsync();
    }
}
