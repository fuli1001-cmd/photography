using MediatR;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands
{
    [DataContract]
    public class PublishPostCommand : IRequest<bool>
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public bool Commentable { get; set; }
        [DataMember]
        public ForwardType ForwardType { get; set; }
        [DataMember]
        public ShareType ShareType { get; set; }
        [DataMember]
        public Visibility Visibility { get; set; }
        [DataMember]
        public Location Location { get; set; }
    }
}
