using AutoMapper;
using Photography.Services.Order.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.API.Query.MapperProfiles
{
    public class OrderViewModelProfile : Profile
    {
        public OrderViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.OrderAggregate.Order, OrderViewModel>();
        }
    }
}
