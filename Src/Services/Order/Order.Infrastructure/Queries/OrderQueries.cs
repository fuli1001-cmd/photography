using Arise.DDD.API.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Order.API.Query.Extensions;
using Photography.Services.Order.API.Query.Interfaces;
using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Order.Infrastructure.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly OrderContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderQueries> _logger;

        public OrderQueries(OrderContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<OrderQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderViewModel> GetOrderAsync(Guid orderId)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableOrders = from o in _dbContext.Orders
                         where o.Id == orderId && (o.User1Id == userId || o.User2Id == userId)
                         select o;

            var queryableDto = GetOrderViewModels(queryableOrders, userId);

            return GetFirstOrderViewModel(await queryableDto.ToListAsync());
        }

        public async Task<OrderViewModel> GetOrderByDealIdAsync(Guid dealId)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableOrders = from o in _dbContext.Orders
                         where o.DealId == dealId && (o.User1Id == userId || o.User2Id == userId)
                         select o;

            var queryableDto = GetOrderViewModels(queryableOrders, userId);

            return GetFirstOrderViewModel(await queryableDto.ToListAsync());
        }

        public async Task<PagedList<OrderViewModel>> GetOrdersAsync(IEnumerable<OrderStatus> orderStatus, PagingParameters pagingParameters)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableOrders = from o in _dbContext.Orders
                         where orderStatus.Contains(o.OrderStatus)
                         && (o.User1Id == userId || o.User2Id == userId)
                         select o;

            var queryableDto = GetOrderViewModels(queryableOrders, userId);

            var pagedDto = await PagedList<OrderViewModel>.ToPagedListAsync(queryableDto, pagingParameters);

            pagedDto.ForEach(o => o.SetAttachmentProperties(_logger));

            return pagedDto;
        }

        private IQueryable<OrderViewModel> GetOrderViewModels(IQueryable<Domain.AggregatesModel.OrderAggregate.Order> orders, Guid userId)
        {
            return from o in orders
                   select new OrderViewModel
                   {
                       Id = o.Id,
                       Price = o.Price,
                       OrderStatus = o.OrderStatus,
                       CreatedTime = o.CreatedTime,
                       UpdatedTime = o.UpdatedTime,
                       AppointedTime = o.AppointedTime,
                       PayerId = o.PayerId,
                       Text = o.Text,
                       Latitude = o.Latitude,
                       Longitude = o.Longitude,
                       LocationName = o.LocationName,
                       Address = o.Address,
                       ClosedTime = o.ClosedTime,
                       Description = o.Description,
                       Attachments = from a in o.Attachments
                                     select new AttachmentViewModel
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         AttachmentStatus = a.AttachmentStatus
                                     },
                       Partner = (from u in _dbContext.Users
                                  where u.Id != userId && (u.Id == o.User1Id || u.Id == o.User2Id)
                                  select new UserViewModel
                                  {
                                      Id = u.Id,
                                      Nickname = u.Nickname,
                                      Avatar = u.Avatar,
                                      UserType = u.UserType,
                                      RealNameRegistrationStatus = u.IdAuthenticated ? IdAuthStatus.Authenticated : IdAuthStatus.NotAuthenticated
                                  }).SingleOrDefault()
                   };
        }

        private OrderViewModel GetFirstOrderViewModel(List<OrderViewModel> orderViewModels)
        {
            if (orderViewModels.Count == 0)
                return null;

            var order = orderViewModels[0];
            order.SetAttachmentProperties(_logger);

            return order;
        }
    }
}
