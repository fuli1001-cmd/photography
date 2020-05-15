using FluentValidation.Internal;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Infrastructure
{
    public class ResponseWrapper
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ResponseWrapper CreateOkResponseWrapper(object data)
        {
            return new ResponseWrapper { Data = data };
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string message)
        {
            return new ResponseWrapper { Code = code, Message = message };
        }
    }
}
