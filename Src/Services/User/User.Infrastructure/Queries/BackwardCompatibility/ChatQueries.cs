using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.User.API.Query.BackwardCompatibility;
using Photography.Services.User.API.Query.Interfaces.BackwardCompatibility;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.BackwardCompatibility.Model;

namespace Photography.Services.User.Infrastructure.Queries.BackwardCompatibility
{
    public class ChatQueries : IChatQueries
    {
        private readonly ChatContext _chatContext;
        private readonly UserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChatQueries> _logger;

        public ChatQueries(
            ChatContext chatContext,
            UserContext userContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChatQueries> logger)
        {
            _chatContext = chatContext ?? throw new ArgumentNullException(nameof(chatContext));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ChatMessage> GetOfflineAndRecentMessagesAsync(string latestMsgId)
        {
            var chatMessage = new ChatMessage();

            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userContext.Users.Where(u => u.Id == myId).Include(u => u.Groups).FirstOrDefaultAsync();
            var chatUserId = user.ChatServerUserId;
            var groupIds = user.Groups.Select(g => g.ChatServerGroupId).ToList();

            chatMessage.OfflineMsgs = (await (from m in _chatContext.PSR_ARS_MessageOffline
                                              where m.IMARSMNRR_GroupId != null
                                              && (((m.IMARSMNRR_FromUserId == chatUserId || m.IMARSMNRR_ToUserId == chatUserId) && groupIds.Contains(m.IMARSMNRR_GroupId.Value))
                                              || (m.IMARSMNRR_ToUserId == chatUserId && m.IMARSMNRR_GroupId == 0))
                                              select new CSInstantMsg
                                              {
                                                  fromUserId = (m.IMARSMNRR_FromUserId ?? 0).ToString(),
                                                  toUserId = (m.IMARSMNRR_ToUserId ?? chatUserId).ToString(),
                                                  chatGroupId = m.IMARSMNRR_GroupId ?? 0,
                                                  msgId = m.IMARSMNRR_MessageId,
                                                  msgType = m.IMARSMNRR_MsgType ?? 11,
                                                  time = m.IMARSMNRR_MsgType ?? 0,
                                                  filenames = m.IMARSMNRR_Filenames,
                                                  thumbnails = m.IMARSMNRR_Thumbnails,
                                                  length = m.IMARSMNRR_Length ?? 0,
                                                  extendData = m.IMARSMNRR_ExtendData,
                                                  isPrivate = m.IMARSMNRR_IsPrivate ?? false,
                                                  content = m.IMARSMNRR_Content
                                              })
                                              .ToListAsync())
                                              .Distinct(new CSInstantMsgComparer());

            var offlinneMessageIds = chatMessage.OfflineMsgs.Select(m => m.msgId);

            if (!string.IsNullOrWhiteSpace(latestMsgId))
            {
                var messages = from c1 in _chatContext.PSR_ARS_Chat
                               from c2 in _chatContext.PSR_ARS_Chat
                               where c2.IMARSC_MsgId == latestMsgId
                               && (c1.IMARSC_Status == null || c1.IMARSC_Status.Value != 1)
                               && !offlinneMessageIds.Contains(c1.IMARSC_MsgId)
                               select new { Chat1 = c1, Chat2 = c2 };

                var personalMessages = from m in messages
                                       where m.Chat1.IMARSC_FromUserId == chatUserId || m.Chat1.IMARSC_ToUserId == chatUserId
                                       select m;

                var groupMessages = from m in messages
                                    where m.Chat1.IMARSC_GroupId != null && groupIds.Contains(m.Chat1.IMARSC_GroupId.Value)
                                    select m;

                var personalMessagesBefore = (from m in personalMessages
                                              orderby m.Chat1.IMARSC_SendTime descending
                                              where m.Chat1.IMARSC_Id < m.Chat2.IMARSC_Id
                                              select m.Chat1)
                                              .Take(10);

                var personalMessagesAfter = (from m in personalMessages
                                             orderby m.Chat1.IMARSC_SendTime descending
                                             where m.Chat1.IMARSC_Id > m.Chat2.IMARSC_Id
                                             select m.Chat1)
                                             .Take(100);


                var groupMessagesBefore = (from m in groupMessages
                                           orderby m.Chat1.IMARSC_SendTime descending
                                           where m.Chat1.IMARSC_Id < m.Chat2.IMARSC_Id
                                           select m.Chat1)
                                           .Take(20);

                var groupMessagesAfter = (from m in groupMessages
                                          orderby m.Chat1.IMARSC_SendTime descending
                                          where m.Chat1.IMARSC_Id > m.Chat2.IMARSC_Id
                                          select m.Chat1)
                                          .Take(20);

                chatMessage.RecentMsgs = (await GetCSInstantMsgs(personalMessagesAfter
                    .Union(personalMessagesBefore)
                    .Union(groupMessagesAfter)
                    .Union(groupMessagesBefore),
                    chatUserId)
                    .ToListAsync())
                    .Distinct(new CSInstantMsgComparer());
            }

            return chatMessage;
        }

        private IQueryable<CSInstantMsg> GetCSInstantMsgs(IQueryable<PSR_ARS_Chat> chats, int chatUserId)
        {
            return from c in chats
                   select new CSInstantMsg
                   {
                       fromUserId = (c.IMARSC_FromUserId ?? 0).ToString(),
                       toUserId = (c.IMARSC_ToUserId ?? chatUserId).ToString(),
                       chatGroupId = c.IMARSC_GroupId ?? 0,
                       msgId = c.IMARSC_MsgId,
                       msgType = c.IMARSC_MessageType ?? 11,
                       time = c.IMARSC_SendTime == null ? (c.IMARSC_SendTime.Value - DateTime.UnixEpoch).TotalSeconds : 0,
                       filenames = c.IMARSC_Pictures,
                       thumbnails = c.IMARSC_Thumbnails,
                       length = c.IMARSC_FileLength ?? 0,
                       extendData = c.IMARSC_ExtendData,
                       isPrivate = c.IMARSC_IsPrivate ?? false,
                       content = c.IMARSC_Message
                   };
        }

        internal class CSInstantMsgComparer : EqualityComparer<CSInstantMsg>
        {
            public override bool Equals(CSInstantMsg x, CSInstantMsg y)
            {
                return x.msgId == y.msgId;
            }

            public override int GetHashCode(CSInstantMsg obj)
            {
                return obj.msgId.GetHashCode();
            }
        }
    }
}
