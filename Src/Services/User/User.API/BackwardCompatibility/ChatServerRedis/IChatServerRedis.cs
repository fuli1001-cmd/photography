using Photography.Services.User.API.Application.Commands.Login;
using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.ChatServerRedis
{
    public interface IChatServerRedis
    {
        Task WriteGroupAsync(Group group);

        Task WriteGroupMemberAsync(Guid userId, int chatServerGroupId, int muted);

        Task RemoveGroupAsync(int chatServerGroupId);

        Task RemoveGroupMemberAsync(Guid userId, int chatServerGroupId);

        Task WriteGroupMessageAsync(Group group, SysMsgType msgType);

        Task WriteGroupMemberMessageAsync(Group group, SysMsgType msgType, IEnumerable<Domain.AggregatesModel.UserAggregate.User> changedUsers, Domain.AggregatesModel.UserAggregate.User operatorUser);

        Task WriteUserAsync(Domain.AggregatesModel.UserAggregate.User user);

        Task<bool> RemoveUserAsync(int chatServerUserId);

        Task WriteTokenUserAsync(Domain.AggregatesModel.UserAggregate.User user, string oldToken);
    }
}
