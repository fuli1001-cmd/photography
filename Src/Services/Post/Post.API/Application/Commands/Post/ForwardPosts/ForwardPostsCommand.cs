using MediatR;
using Photography.Services.Post.API.Application.Commands.Post.PublishPost;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Post.ForwardPosts
{
    [DataContract]
    public class ForwardPostsCommand : BasePostCommand, IRequest<IEnumerable<PostViewModel>>
    {
        public List<Guid> ForwardPostIds { get; set; }
        public bool ShowOriginalText { get; set; }
    }
}
