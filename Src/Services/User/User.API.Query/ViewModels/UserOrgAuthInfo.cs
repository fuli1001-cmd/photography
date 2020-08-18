using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class UserOrgAuthInfo
    {
        public int? OrgType { get; set; }

        // 社团所在高校名称
        public string OrgSchoolName { get; set; }

        // 社团名称
        public string OrgName { get; set; }

        // 认证信息（社团介绍）
        public string OrgDesc { get; set; }

        // 社团运营者姓名
        public string OrgOperatorName { get; set; }

        // 社团运营者手机号
        public string OrgOperatorPhoneNumber { get; set; }

        // 社团照片
        public string OrgImage { get; set; }

        // 社团认证状态
        public IdAuthStatus OrgAuthStatus { get; set; }

        // 社团认证失败原因
        public string OrgAuthMessage { get; set; }
    }
}
