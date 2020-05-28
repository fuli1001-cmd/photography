using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.User.CreateUser
{
    [DataContract]
    public class CreateUserCommand : IRequest<bool>
    {
        [DataMember]
        public string Id { get; set; }
    }
}
