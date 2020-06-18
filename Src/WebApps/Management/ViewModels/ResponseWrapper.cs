using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.WebApps.Management.ViewModels
{
    public class ResponseWrapper<T>
    {
        // 状态码
        public StatusCode Code { get; set; }

        // 显示给用户的消息
        public string Message { get; set; }

        // 开发人员使用的消息
        public List<string> DeveloperMessages { get; set; }

        public T Data { get; set; }
    }

    // 错误状态码
    // reference: https://blog.restcase.com/rest-api-error-codes-101/
    public enum StatusCode
    {
        OK = 0, // 正常
        ClientError = 400, // 客户端错误
        Unauthorized = 401, // 未授权
        ServerError = 500 // 服务端错误
    }
}
