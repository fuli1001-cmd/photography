using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IAppointmentQueries
    {
        Task<List<AppointmentViewModel>> GetAppointmentsAsync();
    }
}
