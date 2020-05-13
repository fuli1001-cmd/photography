using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class PostQueries : IPostQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PostQueries> _logger;

        public PostQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<PostQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PostViewModel>> GetPostsAsync(PostType postType)
        {
            if (postType == PostType.Hot)
                return await GetHotPostsAsync();
            else if (postType == PostType.Followed)
                return await GetFollowedPostsAsync();
            else
                return null;
        }

        private async Task<List<PostViewModel>> GetHotPostsAsync()
        {
            //_postContext.Posts.Select(p =>
            //{
            //    new PostViewModel
            //    {
            //        Id = p.Id,
            //        Text = p.Text,
            //        LikeCount = p.LikeCount,
            //        ShareCount = p.ShareCount,
            //        CommentCount = p.CommentCount,
            //        Timestamp = p.Timestamp,
            //        Commentable = p.Commentable ?? false,
            //        ForwardType = p.ForwardType,
            //        ShareType = p.ShareType,
            //        ViewPassword = p.ViewPassword,
            //        Location = p.Location,
            //        PostAttachments
            //    }
            //})
            var posts = await _postContext.Posts
                .Include(p => p.User)
                .Include(p => p.PostAttachments)
                .Include(p => p.ForwardedPost)
                .OrderByDescending(p => p.Points)
                .ToListAsync();
            return _mapper.Map<List<PostViewModel>>(posts);
        }

        private async Task<List<PostViewModel>> GetFollowedPostsAsync()
        {
            //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //var posts = _postContext.Posts
            //    .Join(_postContext.Users, p => p.UserId, u => u.Id, (p, u) => new { p, u })
            //    .Join(_postContext.UserRelations, pu => pu.p.UserId, ur => ur.FollowedUserId, (pu, ur) => new { pu, ur })
            //    .Where(pur => pur.ur.FollowerId.ToString() == userId)
            //    .GroupJoin(_postContext.Posts, pur => pur.pu.p.ForwardedPostId, fp => fp.Id, (pur, fp) => new { pur, fp })
            //    .GroupJoin(_postContext.PostAttachments, pur_fp => pur_fp.pur.pu.p.Id, pa => pa.PostId, (purfp, pa) => new { purfp, pa })
            //    .SelectMany(pur_fp_pa =>
            //    {
            //        pur_fp_pa.pa.DefaultIfEmpty();
            //        pur_fp_pa.purfp.fp.DefaultIfEmpty();
            //    },
            //    (pur_fp_pa, pa) => new PostViewModel
            //    {
            //        Id = pur_fp_pa.purfp.pur.pu.p.Id,
            //        Text = pur_fp_pa.purfp.pur.pu.p.Text,
            //        LikeCount = pur_fp_pa.purfp.pur.pu.p.LikeCount
            //    });

            ////var posts = await (from p1 in _postContext.Posts
            ////                   join u in _postContext.Users on p1.UserId equals u.Id
            ////                   join ur in _postContext.UserRelations on p1.UserId equals ur.FollowedUserId
            ////                   where ur.FollowerId.ToString() == userId
            ////                   join p2 in _postContext.Posts
            ////                   on p1.ForwardedPostId equals p2.Id
            ////                   into pt
            ////                   from p in pt.DefaultIfEmpty()
            ////                   join a in _postContext.PostAttachments
            ////                   on 
            ////             orderby p1.Points
            ////                   select p1).ToListAsync();

            //return _mapper.Map<List<PostViewModel>>(posts);
            return null;
        }
    }
}
