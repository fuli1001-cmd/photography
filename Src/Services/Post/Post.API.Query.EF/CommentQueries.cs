using Arise.DDD.API.Paging;
using Arise.DDD.API.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class CommentQueries : ICommentQueries
    {
        private readonly PostContext _postContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentQueries> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, 
            IMapper mapper, ILogger<CommentQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedList<CommentViewModel>> GetPostCommentsAsync(Guid postId, int maxSubCommentsCount, PagingParameters pagingParameters)
        {
            var queryableComments = from c in _postContext.Comments
                                    where c.PostId == postId && c.ParentCommentId == null
                                    orderby c.CreatedTime descending
                                    select c;

            var queryableDto = GetSubCommentsViewModelAsync(queryableComments, maxSubCommentsCount);

            return await PagedList<CommentViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<CommentViewModel>> GetSubCommentsAsync(Guid commentId, int maxSubCommentsCount, PagingParameters pagingParameters)
        {
            var queryableComments = from c in _postContext.Comments
                                    where c.ParentCommentId == commentId
                                    orderby c.CreatedTime
                                    select c;

            var queryableDto = GetSubCommentsViewModelAsync(queryableComments, maxSubCommentsCount);

            return await PagedList<CommentViewModel>.ToPagedListAsync(queryableDto, pagingParameters);
        }

        private IQueryable<CommentViewModel> GetSubCommentsViewModelAsync(IQueryable<Comment> queryableComments, int maxSubCommentsCount)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return from c in queryableComments
                   select new CommentViewModel
                   {
                       Id = c.Id,
                       Text = c.Text,
                       Likes = c.Likes,
                       CreatedTime = c.CreatedTime,
                       SubCommentsCount = c.SubComments.Count,
                       SubComments = (from sc in c.SubComments
                                      orderby sc.CreatedTime
                                      select new CommentViewModel
                                      {
                                          Id = sc.Id,
                                          Text = sc.Text,
                                          User = new CommentUserViewModel { Id = sc.User.Id, Nickname = sc.User.Nickname }
                                      }).Take(maxSubCommentsCount),
                       Liked = (from ucr in _postContext.UserCommentRelations
                                where ucr.UserId == userId && ucr.CommentId == c.Id
                                select ucr.Id).Count() > 0,
                       User = new CommentUserViewModel { Id = c.User.Id, Nickname = c.User.Nickname, Avatar = c.User.Avatar }
                   };
        }
    }
}
