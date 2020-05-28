using MediatR;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.UpdateUser
{
    [DataContract]
    public class UpdateUserCommand : IRequest<bool>
    {
        [DataMember]
        [Required]
        public Guid UserId { get; set; }

        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public Gender? Gender { get; set; }

        [DataMember]
        public double? Birthday { get; set; }

        [DataMember]
        public UserType? UserType { get; private set; }

        [DataMember]
        public string Province { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string Sign { get; private set; }
    }
}
