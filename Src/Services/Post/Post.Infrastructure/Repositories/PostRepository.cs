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
            var posts = await _context.Posts.Where(p => p.Id == postId).Include(p => p.AppointmentedUser).ToListAsync();
            
            if (posts.Count > 0)
                return posts[0];
            
            return null;
        }

        public async Task<Domain.AggregatesModel.PostAggregate.Post> GetPostWithAttachmentsById(Guid postId)
        {
            var posts = await _context.Posts.Where(p => p.Id == postId)
                .Include(p => p.PostAttachments)
                .Include(p => p.UserPostRelations)
                .ToListAsync();

            if (posts.Count > 0)
                return posts[0];

            return null;
        }
    }
}
