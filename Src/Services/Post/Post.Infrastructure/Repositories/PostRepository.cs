using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class PostRepository : EfRepository<Domain.AggregatesModel.PostAggregate.Post, PostContext>, IPostRepository
    {
        public PostRepository(PostContext context) : base(context)
        {
            
        }

        public void LoadUser(Domain.AggregatesModel.PostAggregate.Post post)
        {
            _context.Entry(post).Reference(p => p.User).Load();
        }

        public async Task<Domain.AggregatesModel.PostAggregate.Post> GetPostWithAppointmentedUserById(Guid postId)
        {
            return await _context.Posts.Where(p => p.Id == postId)
                .Include(p => p.AppointmentedUser)
                .SingleOrDefaultAsync();
        }

        public async Task<Domain.AggregatesModel.PostAggregate.Post> GetPostWithAttachmentsById(Guid postId)
        {
            return await _context.Posts.Where(p => p.Id == postId)
                .Include(p => p.PostAttachments)
                .Include(p => p.UserPostRelations)
                .SingleOrDefaultAsync();
        }

        public async Task<Domain.AggregatesModel.PostAggregate.Post> GetPostWithNavigationPropertiesById(Guid postId)
        {
            return await _context.Posts.Where(p => p.Id == postId)
                .Include(p => p.PostAttachments)
                .Include(p => p.Comments)
                .Include(p => p.ForwardingPosts)
                .SingleOrDefaultAsync();
        }

        public async Task<Domain.AggregatesModel.PostAggregate.Post> GetAppointmentById(Guid postId)
        {
            return await _context.Posts.Where(p => p.Id == postId)
                .Include(p => p.PostAttachments)
                .Include(p => p.AppointmentedFromPosts)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetPostCommentCountAsync(Guid postId)
        {
            return await (from p in _context.Posts
                          where p.Id == postId
                          select p.CommentCount)
                   .SingleOrDefaultAsync();
        }
    }
}
