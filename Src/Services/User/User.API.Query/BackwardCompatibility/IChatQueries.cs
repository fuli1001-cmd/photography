using Photography.Services.User.API.Query.BackwardCompatibility;
using Photography.Services.User.Domain.BackwardCompatibility.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces.BackwardCompatibility
{
    public interface IChatQueries
    {
        Task<ChatMessage> GetOfflineAndRecentMessagesAsync(string latestMsgId);

        Task<List<PSR_ARS_ErrorCode>> GetErrorCodesAsync();
    }
}
