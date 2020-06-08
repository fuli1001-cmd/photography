using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.UpdateGroup
{
    public class UpdateGroupCommand : IRequest<bool>
    {
        [Required]
        public Guid GroupId { get; set; }

        public string Notice { get; set; }

        public string Avatar { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
