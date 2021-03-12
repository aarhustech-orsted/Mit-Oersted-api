using System;
using System.Globalization;
using System.Net;

namespace Mit_Oersted.Domain.ErrorHandling
{
    public static class ExceptionFactory
    {
        public static Exception MappingOfRecipeFailed()
        {
            return new DomainException(
                string.Format(CultureInfo.InvariantCulture, "There was an error with mapping from one model to another"),
                ErrorCodes.MappingOfRecipeFailed,
                HttpStatusCode.InternalServerError);
        }

        public static Exception CommandIsNull()
        {
            return new DomainException(
                string.Format(CultureInfo.InvariantCulture, "The command was null"),
                ErrorCodes.CommandIsNull,
                HttpStatusCode.InternalServerError);
        }
    }
}
