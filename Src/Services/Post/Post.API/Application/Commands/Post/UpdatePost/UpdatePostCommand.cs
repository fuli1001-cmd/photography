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
    }
}
