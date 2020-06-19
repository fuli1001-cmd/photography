using Photography.Services.Notification.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Query.Interfaces
{
    public interface IUserQueries
    {
        Task<PushSettingsViewModel> GetPushSettingsAsync();
    }
}
