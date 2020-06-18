using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.WebApps.Management.ViewModels
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
