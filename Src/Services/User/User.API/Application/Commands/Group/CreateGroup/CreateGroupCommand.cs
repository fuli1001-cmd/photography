using MediatR;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.CreateGroup
{
    public class CreateGroupCommand : IRequest<GroupViewModel>
    {
        [Required]
        public string Name { get; set; }

        public string Avatar { get; set; }

        public List<Guid> MemberIds { get; set; }
    }
}
