using MediatR;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public UserType? UserType { get; set; }
    }
}
