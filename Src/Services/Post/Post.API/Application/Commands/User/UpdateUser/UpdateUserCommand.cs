using MediatR;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.UpdateUser
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public UserType? UserType { get; set; }
    }
}
