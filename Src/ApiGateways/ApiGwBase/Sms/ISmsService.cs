using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.ApiGateways.ApiGwBase.Sms
{
    public interface ISmsService
    {
        string SendSms(string Phonenumber);
    }
}
