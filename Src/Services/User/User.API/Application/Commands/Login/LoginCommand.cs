using MediatR;
using Photography.Services.User.API.BackwardCompatibility.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Login
{
    [DataContract]
    public class LoginCommand : IRequest<TokensViewModel>
    {
        [DataMember]
        [Required]
        public string UserName { get; set; }

        [DataMember]
        [Required]
        public string Password { get; set; }

        [DataMember]
        [Required]
        public int ClientType { get; set; }

        [DataMember]
        public string RegistrationId { get; set; }
    }
}
