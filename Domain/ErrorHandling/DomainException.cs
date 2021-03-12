using System;
using System.Net;

namespace Mit_Oersted.Domain.ErrorHandling
{
    public class DomainException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public string ErrorCode { get; private set; }

        public DomainException(
            string message,
            string errorCode,
            HttpStatusCode statusCode
            ) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
