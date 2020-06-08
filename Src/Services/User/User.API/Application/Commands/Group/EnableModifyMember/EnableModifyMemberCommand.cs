using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.EnableModifyMember
{
    /// <summary>
    /// 允许群成员加人
    /// </summary>
    public class EnableModifyMemberCommand : IRequest<bool>
    {
        public Guid GroupId { get; set; }

        // 打开或关闭允许群成员加人的权限
        public bool Enabled { get; set; }
    }
}
