using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
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

        public async Task<List<AppointmentViewModel>> GetReceivedAppointmentDealsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var deals = await _postContext.Posts
                .Where(p => p.AppointmentedUserId.ToString() == userId && p.PostType == PostType.AppointmentDeal && p.AppointmentDealStatus == AppointmentDealStatus.Created)
                .OrderBy(p => p.AppointedTime)
                .Include(p => p.PostAttachments)
                .Include(p => p.User)
                .ToListAsync();

            var vms = _mapper.Map<List<AppointmentViewModel>>(deals);

            // 设置付款方，由于这里查询的是收到的约拍交易，支付视角相对于发出的约拍交易是反的,
            // 而约拍交易是由交易发出人创建的，所以支付方需要对调一下。
            vms.ForEach(vm =>
            {
                if (vm.PayerType == PayerType.Me)
                    vm.PayerType = PayerType.You;
                else if (vm.PayerType == PayerType.You)
                    vm.PayerType = PayerType.Me;
            });

            return vms;
        }

        public async Task<List<AppointmentViewModel>> GetSentAppointmentDealsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var deals = await _postContext.Posts
                .Where(p => p.UserId.ToString() == userId && p.PostType == PostType.AppointmentDeal && p.AppointmentDealStatus == AppointmentDealStatus.Created)
                .OrderBy(p => p.AppointedTime)
                .Include(p => p.PostAttachments)
                .Include(p => p.AppointmentedUser)
                .ToListAsync();

            var dealsViewModel = _mapper.Map<List<AppointmentViewModel>>(deals);
            dealsViewModel.ForEach(dvm =>
            {
                var deal = deals.FirstOrDefault(d => d.Id == dvm.Id);
                dvm.User = new AppointmentUserViewModel
                {
                    Id = deal.AppointmentedUser.Id,
                    Nickname = deal.AppointmentedUser.Nickname,
                    Avatar = deal.AppointmentedUser.Avatar,
                    UserType = deal.AppointmentedUser.UserType,
                    Score = deal.AppointmentedUser.Score
                };
            });

            return dealsViewModel;
        }
    }
}
