using Arise.DDD.API.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Queries
{
    public class CommentQueries : ICommentQueries
    {
        private readonly PostContext _postContext;
        private readonly ILogger<CommentQueries> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentQueries(PostContext postContext, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<CommentQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            Guid myId = Guid.Empty;

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
                myId = Guid.Parse(claim.Value);

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
                                          CreatedTime = sc.CreatedTime,
                                          User = new CommentUserViewModel { Id = sc.User.Id, Nickname = sc.User.Nickname, Avatar = sc.User.Avatar, OrgAuthStatus = sc.User.OrgAuthStatus }
                                      }).Take(maxSubCommentsCount),
                       Liked = (from ucr in _postContext.UserCommentRelations
                                where ucr.UserId == myId && ucr.CommentId == c.Id
                                select ucr.Id).Count() > 0,
                       User = new CommentUserViewModel { Id = c.User.Id, Nickname = c.User.Nickname, Avatar = c.User.Avatar, OrgAuthStatus = c.User.OrgAuthStatus }
                   };
        }
    }
}
