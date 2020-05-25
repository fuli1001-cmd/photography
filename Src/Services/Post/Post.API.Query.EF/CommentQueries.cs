using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class CommentQueries : ICommentQueries
    {
        private readonly PostContext _postContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentQueries> _logger;

        public CommentQueries(PostContext postContext, IMapper mapper, ILogger<CommentQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CommentViewModel>> GetPostCommentsAsync(Guid postId)
        {
            var comments = await _postContext.Comments.Where(c => c.PostId != null && c.PostId.Value == postId)
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .OrderByDescending(c => c.CreatedTime)
                .ToListAsync();

            return _mapper.Map<List<CommentViewModel>>(comments);
        }
    }
}
