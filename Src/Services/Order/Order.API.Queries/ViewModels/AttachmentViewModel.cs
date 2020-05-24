using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.API.Query.ViewModels
{
    public class AttachmentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public AttachmentStatus AttachmentStatus { get; set; }
    }
}
