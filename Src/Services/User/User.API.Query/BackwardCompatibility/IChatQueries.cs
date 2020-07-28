using Photography.Services.User.API.Query.BackwardCompatibility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Query.Interfaces.BackwardCompatibility
{
    public interface IChatQueries
    {
        Task<ChatMessage> GetOfflineAndRecentMessagesAsync(string latestMsgId);
    }
}
