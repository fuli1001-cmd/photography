using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class PostQueries : IPostQueries
    {
        private readonly PostContext _postContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PostQueries> _logger;

        public PostQueries(PostContext postContext, IMapper mapper, ILogger<PostQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<PostViewModel> GetPostsAsync(PostType postType)
        {
            if (postType == PostType.Hot)
                return GetHotPostsAsync();
            else
                return null;
        }

        private List<PostViewModel> GetHotPostsAsync()
        {
            var posts = _postContext.Posts
                .Include(p => p.User)
                .Include(p => p.PostAttachments)
                .OrderByDescending(p => p.Points)
                .ToList();
            return _mapper.Map<List<PostViewModel>>(posts);
        }
    }
}
