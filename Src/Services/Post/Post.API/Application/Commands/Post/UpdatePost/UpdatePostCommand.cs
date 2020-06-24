using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.UpdatePost
{
    [DataContract]
    public class UpdatePostCommand : PublishPostCommand
    {
        [DataMember]
        [Required]
        public Guid PostId { get; set; }

        // 编辑转发的帖子时，需传此字段，编辑非转发帖子时，不需传
        public bool? ShowOriginalText { get; set; }
    }
}
