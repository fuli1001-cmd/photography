using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.DeletePost
{
    [DataContract]
    public class DeletePostCommand : IRequest<bool>
    {
        [DataMember]
        [Required]
        public Guid PostId { get; set; }
    }
}
