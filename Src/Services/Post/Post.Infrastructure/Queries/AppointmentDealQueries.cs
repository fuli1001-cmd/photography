using Arise.DDD.API.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Infrastructure.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Queries
{
    public class AppointmentDealQueries : IAppointmentDealQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentDealQueries> _logger;

        public AppointmentDealQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<AppointmentDealQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedList<AppointmentViewModel>> GetReceivedAppointmentDealsAsync(PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users 
                                     on p.UserId equals u.Id
                                     where p.AppointmentedUserId == myId && p.PostType == PostType.AppointmentDeal && p.AppointmentDealStatus == AppointmentDealStatus.Created
                                     orderby p.AppointedTime
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryableAppointmentViewModels(queryableUserPosts);

            var pagedDto = await GetPagedAppointmentViewModelsAsync(queryableDto, pagingParameters);

            // 设置付款方，由于这里查询的是收到的约拍交易，支付视角相对于发出的约拍交易是反的,
            // 而约拍交易是由交易发出人创建的，所以支付方需要对调一下。
            pagedDto.ForEach(vm =>
            {
                if (vm.PayerType == PayerType.Me)
                    vm.PayerType = PayerType.You;
                else if (vm.PayerType == PayerType.You)
                    vm.PayerType = PayerType.Me;
            });

            return pagedDto;
        }

        public async Task<PagedList<AppointmentViewModel>> GetSentAppointmentDealsAsync(PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.AppointmentedUserId equals u.Id
                                     where p.UserId == myId && p.PostType == PostType.AppointmentDeal && p.AppointmentDealStatus == AppointmentDealStatus.Created
                                     orderby p.AppointedTime
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryableAppointmentViewModels(queryableUserPosts);

            var pagedDto = await GetPagedAppointmentViewModelsAsync(queryableDto, pagingParameters);

            return pagedDto;
        }

        public async Task<AppointmentViewModel> GetSentAppointmentDealAsync(Guid dealId)
        {
            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.AppointmentedUserId equals u.Id
                                     where p.Id == dealId
                                     select new UserPost { Post = p, User = u };

            return await GetQueryableAppointmentViewModels(queryableUserPosts).FirstOrDefaultAsync();
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
                           RealNameRegistrationStatus = up.User.IdAuthenticated ? IdAuthStatus.Authenticated : IdAuthStatus.NotAuthenticated
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
            pagedDto.ForEach(dto =>
            {
                foreach (var attachment in dto.PostAttachments)
                    attachment.SetProperties();
            });

            return pagedDto;
        }
    }
}
