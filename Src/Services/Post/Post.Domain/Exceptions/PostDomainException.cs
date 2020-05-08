using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Exceptions
{
    public class PostDomainException : Exception
    {
        public PostDomainException()
        { }

        public PostDomainException(string message)
            : base(message)
        { }

        public PostDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
