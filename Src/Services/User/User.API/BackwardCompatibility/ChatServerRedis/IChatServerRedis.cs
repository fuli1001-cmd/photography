using Photography.Services.User.API.BackwardCompatibility.Models;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.ChatServerRedis
{
    public interface IChatServerRedis
    {
        //Task WriteGroupToRedisAsync(Group group);

        //Task WriteGroupMemberToRedisAsync(Guid userId, int chatServerGroupId);

        //Task WriteGroupMessageToRedisAsync(Group group, SysMsgType msgType);

        //Task WriteGroupMemberMessageToRedisAsync(Group group, SysMsgType msgType);
    }
}
