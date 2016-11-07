using System;
using System.Net;

namespace ApiCalls.Contracts
{
    public class ControllerProxyException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ControllerProxyException()
        {}

        public ControllerProxyException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
