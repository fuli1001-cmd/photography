using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Dtos
{
    public class LoginPhoneDto
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }

        public int ClientType { get; set; }

        public string RegistrationId { get; set; }
    }
}
