using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Query.ViewModels
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public double CreatedTime { get; set; }

        public double UpdatedTime { get; set; }

        public double AppointedTime { get; set; }

        public Guid? PayerId { get; set; }

        public string Text { get; set; }

        public double ClosedTime { get; set; }

        public string Description { get; set; }

        public UserViewModel Partner { get; set; }

        public IEnumerable<AttachmentViewModel> Attachments { get; set; }

        #region location properties
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LocationName { get; set; }

        public string Address { get; set; }

        public string CityCode { get; set; }
        #endregion
    }
}
