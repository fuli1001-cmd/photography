using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.ToggleFollow
{
    [DataContract]
    public class ToggleFollowCommand : IRequest<bool>
    {
        /// <summary>
        /// 要follow的用户id
        /// </summary>
        [DataMember]
        [Required]
        public Guid UserIdToFollow { get; set; }
    }
}
