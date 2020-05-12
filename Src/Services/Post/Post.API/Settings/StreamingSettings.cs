using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Settings
{
    public class StreamingSettings
    {
        public long FileSizeLimit { get; set; }
        public string StoredFilesPath { get; set; }
    }
}
