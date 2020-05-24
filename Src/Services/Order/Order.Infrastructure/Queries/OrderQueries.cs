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
        private readonly IMapper _mapper;
        private readonly ILogger<OrderQueries> _logger;

        public OrderQueries(OrderContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<OrderQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<OrderViewModel>> GetOrdersAsync(OrderStatus orderStatus)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orders = from o in _dbContext.Orders
                     where o.OrderStatus == orderStatus
                     && (o.User1Id.ToString() == userId || o.User2Id.ToString() == userId)
                     select new OrderViewModel
                     {
                         Id = o.Id,
                         Price = o.Price,
                         OrderStatus = o.OrderStatus,
                         CreatedTime = o.CreatedTime,
                         AppointedTime = o.AppointedTime,
                         PayerId = o.PayerId,
                         Text = o.Text,
                         Latitude = o.Latitude,
                         Longitude = o.Longitude,
                         LocationName = o.LocationName,
                         Address = o.Address,
                         Attachments = from a in o.Attachments
                                       select new AttachmentViewModel
                                       {
                                           Id = a.Id,
                                           Name = a.Name,
                                           AttachmentStatus = a.AttachmentStatus
                                       },
                         Partner = (from u in _dbContext.Users
                                    where u.Id.ToString() != userId && (u.Id == o.User1Id || u.Id == o.User2Id)
                                    select new UserViewModel
                                    {
                                        Id = u.Id,
                                        Nickname = u.Nickname,
                                        Avatar = u.Avatar,
                                        UserType = u.UserType
                                    }).SingleOrDefault()
                     };
            
            var orderList = await orders.ToListAsync();
            orderList.ForEach(o => o.SetAttachmentProperties(_logger));

            return orderList;
        }
    }
}
