using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.ChangeGroupOwner
{
    public class ChangeGroupOwnerCommand : IRequest<bool>
    {
        [Required]
        public Guid GroupId { get; set; }

        /// <summary>
        /// 新的群主id
        /// </summary>
        [Required]
        public Guid NewOwnerId { get; set; }
    }
}
