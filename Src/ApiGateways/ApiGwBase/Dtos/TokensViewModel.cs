using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Dtos
{
    public class TokensViewModel
    {
        public string AccessToken { get; set; }
        public string OldToken { get; set; }
    }
}
