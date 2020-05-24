using MediatR;
using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.AppointmentDeal.AppointTask
{
    [DataContract]
    public class AppointTaskCommand : IRequest<bool>
    {
        /// <summary>
        /// 要约拍的帖子Id
        /// </summary>
        [DataMember]
        [Required]
        public Guid AppointmentId { get; set; }

        /// <summary>
        /// 约拍描述
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [DataMember]
        public List<Attachment> Attachments { get; set; }
    }
}
