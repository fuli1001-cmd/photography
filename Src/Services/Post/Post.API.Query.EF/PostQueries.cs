using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class PostQueries : IPostQueries
    {
        private readonly PostContext _postContext;
        private readonly ILogger<PostQueries> _logger;

        public PostQueries(PostContext postContext, ILogger<PostQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<PostViewModel> GetPostsAsync(PostType postType)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<PostViewModel> GetHotPostsAsync()
        {
            return _postContext.Posts
                .Include(p => p.User)
                .Include(p => p.PostAttachments)
                .OrderByDescending(p => p.Points)
                .Select(p => p.ToPostViewModel());
        }
    }
}
