using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public enum SysMsgType : byte
    {
        GROUP_CREATED = 1,
        GROUP_DISMISSED = 2,
        ADDED_A_GROUP = 3,
        REMOVED_FROM_GROUP = 4,
        SECEDE_FROM_GROUP = 5,
        NEW_MEMBER_ADDED = 6,
        MEMBER_SECEDED = 7,
        GROUP_INFO_CHANGED = 8,
        OWNER_CHANGED = 9,
        MSG_REVOKED = 10,
        FRIEND_REQ_APPROVED = 11
    }
}
