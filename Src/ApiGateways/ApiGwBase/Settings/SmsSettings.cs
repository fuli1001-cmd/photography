﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.ApiGateways.ApiGwBase.Settings
{
    public class SmsSettings
    {
        public string Key { get; set; }
        public string Secrect { get; set; }
        public string Domain { get; set; }
        public string Version { get; set; }
        public string Action { get; set; }
        public string SignName { get; set; }
        public string TemplateCode { get; set; }
    }
}
