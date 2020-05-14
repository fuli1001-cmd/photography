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
        public string ViewPassword { get; set; }

        [DataMember]
        public List<Guid> friendIds { get; set; }

        [DataMember]
        public List<Attachment> attachments { get; set; }


        [DataMember]
        public string Province { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public string LocationName { get; set; }

        [DataMember]
        public string Address { get; set; }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
