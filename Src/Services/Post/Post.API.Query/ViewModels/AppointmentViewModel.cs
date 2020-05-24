﻿using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class AppointmentViewModel : BasePostViewModel
    {
        public double Timestamp { get; set; }
        public double AppointedTime { get; set; }
        public decimal Price { get; set; }
        public PayerType PayerType { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; private set; }
        public string Address { get; private set; }
        public AppointmentUserViewModel User { get; set; }
    }

    public class AppointmentDealViewModel : AppointmentViewModel
    {
        public AppointmentDealStatus AppointmentDealStatus { get; set; }
    }
}
