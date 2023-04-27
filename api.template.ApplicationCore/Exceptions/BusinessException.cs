using System;
using System.Net;

namespace Api.Template.ApplicationCore.Exceptions
{
    public class BusinessException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public BusinessException()
        {
        }

        public BusinessException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public BusinessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}