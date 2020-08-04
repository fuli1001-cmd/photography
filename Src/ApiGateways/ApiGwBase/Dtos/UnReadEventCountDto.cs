using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Dtos
{
    /// <summary>
    /// 未读事件及其数量
    /// </summary>
    public class UnReadEventCountDto
    { 
        /// <summary>
        /// 互动事件未读数量
        /// </summary>
        public int Interaction { get; set; }

        /// <summary>
        /// 约拍事件未读数量
        /// </summary>
        public int Appointment { get; set; }

        /// <summary>
        /// 系统事件未读数量
        /// </summary>
        public int System { get; set; }

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
