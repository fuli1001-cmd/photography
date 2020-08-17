using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.AuthOrg
{
    /// <summary>
    /// 团体认证命令
    /// </summary>
    public class AuthOrgCommand : IRequest<bool>
    {
        /// <summary>
        /// 社团类型
        /// </summary>
        public int OrgType { get; set; }

        /// <summary>
        /// 社团所在高校名称
        /// </summary>
        public string OrgSchoolName { get; set; }

        /// <summary>
        /// 社团名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 认证信息
        /// </summary>
        public string OrgDesc { get; set; }

        /// <summary>
        /// 社团运营者姓名
        /// </summary>
        public string OrgOperatorName { get; set; }

        /// <summary>
        /// 社团运营者手机号
        /// </summary>
        public string OrgOperatorPhoneNumber { get; set; }

        /// <summary>
        /// 社团照片
        /// </summary>
        public string OrgImage { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string Code { get; set; }
    }
}
