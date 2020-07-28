using Arise.DDD.API.Paging;
using Arise.DDD.Domain.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Queries
{
    public class AppointmentQueries : IAppointmentQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentQueries> _logger;

        public AppointmentQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<AppointmentQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 获取约拍广场的约拍列表
        // 返回与当前用户不同类型的用户发的约拍
        // 以及当前用户发的约拍
        public async Task<PagedList<AppointmentViewModel>> GetAppointmentsAsync(PayerType? payerType, double? appointmentSeconds, PagingParameters pagingParameters)
        {
            IQueryable<UserPost> queryableUserPosts;

            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (role != "admin")
            {
                var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var curUserType = _postContext.Users.SingleOrDefault(u => u.Id == myId)?.UserType ?? throw new ClientException("操作失败", new List<string> { $"The type of user {myId} is not set." });

                // 与当前用户不同类型的用户所发的约拍及当前用户发的约拍
                queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users on p.UserId equals u.Id
                                     where p.PostType == PostType.Appointment && (u.UserType != curUserType || p.UserId == myId)
                                     orderby p.CreatedTime descending
                                     select new UserPost { Post = p, User = u };
            }
            else
            {
                queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users on p.UserId equals u.Id
                                     where p.PostType == PostType.Appointment
                                     orderby p.CreatedTime descending
                                     select new UserPost { Post = p, User = u };
            }

            // 筛选支付方类型
            if (payerType != null)
                queryableUserPosts = queryableUserPosts.Where(up => up.Post.PayerType == payerType);

            // 筛选指定日期当天的约拍
            if (appointmentSeconds != null)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
                var date = DateTime.UnixEpoch.AddSeconds(appointmentSeconds.Value);
                var startSeconds = (new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0) - epoch).TotalSeconds;
                var endSeconds = (new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999) - epoch).TotalSeconds;
                queryableUserPosts = queryableUserPosts.Where(up => up.Post.AppointedTime != null && up.Post.AppointedTime.Value >= startSeconds && up.Post.AppointedTime.Value <= endSeconds);
            }

            var queryableDto = GetQueryableAppointmentViewModels(queryableUserPosts);

            return await GetPagedAppointmentViewModelsAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<AppointmentViewModel>> GetMyAppointmentsAsync(PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users on p.UserId equals u.Id
                                     where p.PostType == PostType.Appointment && p.UserId == myId
                                     orderby p.CreatedTime descending
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryableAppointmentViewModels(queryableUserPosts);

            return await GetPagedAppointmentViewModelsAsync(queryableDto, pagingParameters);
        }

        public async Task<AppointmentViewModel> GetAppointmentAsync(Guid appointmentId)
        {
            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users on p.UserId equals u.Id
                                     where p.Id == appointmentId
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryableAppointmentViewModels(queryableUserPosts);

            return await queryableDto.SingleOrDefaultAsync();
        }

        private IQueryable<AppointmentViewModel> GetQueryableAppointmentViewModels(IQueryable<UserPost> queryableUserPosts)
        {
            return from up in queryableUserPosts
                   select new AppointmentViewModel
                   {
                       Id = up.Post.Id,
                       Text = up.Post.Text,
                       CreatedTime = up.Post.CreatedTime,
                       AppointedTime = up.Post.AppointedTime.Value,
                       Price = up.Post.Price ?? 0,
                       PayerType = up.Post.PayerType.Value,
                       CityCode = up.Post.CityCode,
                       Latitude = up.Post.Latitude.Value,
                       Longitude = up.Post.Longitude.Value,
                       LocationName = up.Post.LocationName,
                       Address = up.Post.Address,
                       User = new AppointmentUserViewModel
                       {
                           Id = up.User.Id,
                           Nickname = up.User.Nickname,
                           Avatar = up.User.Avatar,
                           UserType = up.User.UserType,
                           Score = up.User.Score,
                           RealNameRegistrationStatus = up.User.IdAuthenticated ? IdAuthStatus.Authenticated : IdAuthStatus.NotAuthenticated,
                       },
                       PostAttachments = from a in up.Post.PostAttachments
                                         select new PostAttachmentViewModel
                                         {
                                             Id = a.Id,
                                             Name = a.Name,
                                             Text = a.Text,
                                             AttachmentType = a.AttachmentType
                                         }
                   };
        }

        private async Task<PagedList<AppointmentViewModel>> GetPagedAppointmentViewModelsAsync(IQueryable<AppointmentViewModel> queryableDto, PagingParameters pagingParameters)
        {
            var pagedDto = await PagedList<AppointmentViewModel>.ToPagedListAsync(queryableDto, pagingParameters);

            // 设置附件属性：宽、高、视频缩略图
            pagedDto.ForEach(a => a.SetAttachmentProperties(_logger));

            return pagedDto;
        }

        class UserPost
        {
            public Domain.AggregatesModel.PostAggregate.Post Post { get; set; }
            public User User { get; set; }
        }
    }
}
