using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.ModifyGroupMembers
{
    public class ModifyGroupMembersCommand : IRequest<bool>
    {
        [Required]
        public Guid GroupId { get; set; }
        
        // 要删除的成员id
        public List<Guid> RemovedMemberIds { get; set; }

        // 要新增的成员id
        public List<Guid> NewMemberIds { get; set; }
    }
}
