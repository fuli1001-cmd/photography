using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class AppointmentDealCreatedEvent : BaseEvent
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }        
        // 该笔交易的id
        public Guid DealId { get; set; }
        public decimal Price { get; set; }
        public double AppointedTime { get; set; }
        public Guid? PayerId { get; set; }
        public string Text { get; set; }

        #region location properties
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LocationName { get; set; }

        public string Address { get; set; }
        #endregion
    }
}
