using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.BackwardCompatibility.Model
{
    public class PSR_ARS_ErrorCode
    {
        public int uid { get; set; }
        public int code { get; set; }
        public string ch { get; set; }
        public string en { get; set; }
        public string desc { get; set; }
    }
}
