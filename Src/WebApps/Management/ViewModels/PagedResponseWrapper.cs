using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.WebApps.Management.ViewModels
{
    public class PagedResponseWrapper<T> : ResponseWrapper<T>
    {
        public PagingInfo PagingInfo { get; set; }
    }
}
