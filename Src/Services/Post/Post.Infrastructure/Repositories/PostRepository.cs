using Arise.DDD.Infrastructure;
using Microsoft.Data.SqlClient;
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

        //public async Task<Dictionary<Guid, Guid>> GetPostsUserIdsAsync(List<Guid> postIds)
        //{
        //    var dic = new Dictionary<Guid, Guid>();
        //    var posts = await _context.Posts.Where(p => postIds.Contains(p.Id)).ToListAsync();
        //    posts.ForEach(p => dic.Add(p.Id, p.UserId));
        //    return dic;
        //}

        public async Task<List<Domain.AggregatesModel.PostAggregate.Post>> GetPostsAsync(List<Guid> postIds)
        {
            return await (from p in _context.Posts
                          where postIds.Contains(p.Id)
                          select p)
                          .Include(p => p.UserPostRelations)
                          .ToListAsync();
        }

        public async Task<List<Domain.AggregatesModel.PostAggregate.Post>> GetUserPostsByPrivateTag(Guid userId, string privateTag)
        {
            return await _context.Posts.Where(p => p.UserId == userId && p.PrivateTag.ToLower() == privateTag.ToLower() && p.PostType == PostType.Post).ToListAsync();
        }

        public async Task<List<Domain.AggregatesModel.PostAggregate.Post>> GetUserPostsAsync(Guid userId)
        {
            return await _context.Posts.Where(p => p.UserId == userId && p.PostType == PostType.Post).ToListAsync();
        }

        public async Task RefreshPostScore(int startRefreshHour, int refreshIntervalHour, double percent)
        {
            var nowInSeconds = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

            var commandBuilder = new StringBuilder();
            commandBuilder.Append("update Posts set Score = Score * @Percent, LastScoreRefreshedTime = @NowInSeconds ");
            commandBuilder.Append("where PostType = 0 ");
            commandBuilder.Append("and (@NowInSeconds - CreatedTime) > @StartRefreshHour * 3600 ");
            commandBuilder.Append("and (@NowInSeconds - LastScoreRefreshedTime) > @RefreshIntervalHour * 3600");

            var paramPercent = new SqlParameter("@Percent", percent);
            var paramNowInSeconds = new SqlParameter("@NowInSeconds", nowInSeconds);
            var paramStartRefreshHour = new SqlParameter("@StartRefreshHour", startRefreshHour);
            var paramRefreshIntervalHour = new SqlParameter("@RefreshIntervalHour", refreshIntervalHour);

            await _context.Database.ExecuteSqlRawAsync(commandBuilder.ToString(), paramPercent, paramNowInSeconds, paramStartRefreshHour, paramRefreshIntervalHour);
        }

        public async Task<int> GetTodayUserSentAppointmentDealCountAsync(Guid userId)
        {
            var startSeconds = GetTodayStartSeconds();
            var endSeconds = GetTodayEndSeconds();

            return await _context.Posts.Where(p => p.PostType == PostType.AppointmentDeal && p.CreatedTime <= endSeconds && p.CreatedTime >= startSeconds && p.UserId == userId).CountAsync();
        }

        public async Task<int> GetTodayUserReceivedAppointmentDealCountAsync(Guid userId)
        {
            var startSeconds = GetTodayStartSeconds();
            var endSeconds = GetTodayEndSeconds();

            return await _context.Posts.Where(p => p.PostType == PostType.AppointmentDeal && p.CreatedTime <= endSeconds && p.CreatedTime >= startSeconds && p.AppointmentedUserId == userId).CountAsync();
        }

        public async Task<bool> UserHasAppointmentTodayAsync(Guid userId)
        {
            var startSeconds = GetTodayStartSeconds();
            var endSeconds = GetTodayEndSeconds();

            return await _context.Posts.AnyAsync(p => p.PostType == PostType.Appointment && p.UserId == userId && p.CreatedTime <= endSeconds && p.CreatedTime >= startSeconds);
        }

        // 获取今天开始时间的时间戳
        private double GetTodayStartSeconds()
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            return (startTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 获取今天截止时间的时间戳
        private double GetTodayEndSeconds()
        {
            var endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            return (endtime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
    }
}
