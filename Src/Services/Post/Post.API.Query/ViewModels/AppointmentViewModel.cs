using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class AppointmentViewModel : BasePostViewModel
    {
        public double CreatedTime { get; set; }
        public double AppointedTime { get; set; }
        public decimal Price { get; set; }
        public PayerType PayerType { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public AppointmentUserViewModel User { get; set; }
    }

    /// <summary>
    /// 发出和收到的约拍交易数量
    /// </summary>
    public class SentAndReceivedAppointmentDealCountViewModel
    {
        /// <summary>
        /// 收到的约拍数量
        /// </summary>
        public int ReceivedAppointmentDeal { get; set; }

        /// <summary>
        /// 发出的约拍数量
        /// </summary>
        public int SentAppointmentDeal { get; set; }
    }
}
