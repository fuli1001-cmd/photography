using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Login
{
    [DataContract]
    public class LoginCommand : IRequest<string>
    {
        [DataMember]
        [Required]
        public string UserName { get; set; }

        [DataMember]
        [Required]
        public string Password { get; set; }
    }
}
