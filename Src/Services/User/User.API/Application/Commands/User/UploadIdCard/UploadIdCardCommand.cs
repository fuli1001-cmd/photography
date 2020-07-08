using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.UploadIdCard
{
    /// <summary>
    /// 上传身份证
    /// </summary>
    public class UploadIdCardCommand : IRequest<bool>
    {
        /// <summary>
        /// 身份证正面
        /// </summary>
        public string IdCardFront { get; set; }

        /// <summary>
        /// 身份证背面
        /// </summary>
        public string IdCardBack { get; set; }

        /// <summary>
        /// 手持身份证
        /// </summary>
        public string IdCardHold { get; set; }
    }
}
