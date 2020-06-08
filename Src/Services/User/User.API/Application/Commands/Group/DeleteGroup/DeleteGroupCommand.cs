using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.DeleteGroup
{
    public class DeleteGroupCommand : IRequest<bool>
    {
        [Required]
        public Guid GroupId { get; set; }
    }
}
