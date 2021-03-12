using System;
using System.Globalization;
using System.Net;

namespace Mit_Oersted.Domain.ErrorHandling
{
    public static class ExceptionFactory
    {
        public static Exception MappingOfRecipeFailedException()
        {
            return new DomainException(
                string.Format(CultureInfo.InvariantCulture, "There was an error with mapping from one model to another"),
                ErrorCodes.MappingOfRecipeFailed,
                HttpStatusCode.InternalServerError);
        }

        public static Exception CommandIsNullException()
        {
            return new DomainException(
                string.Format(CultureInfo.InvariantCulture, "The command was null"),
                ErrorCodes.CommandIsNull,
                HttpStatusCode.InternalServerError);
        }

        public static Exception ErrorWithGoogleAuthException(string httpMessage)
        {
            return new DomainException(
                string.Format(CultureInfo.InvariantCulture, $"There was an error with the communicating to Google. { httpMessage }"),
                ErrorCodes.ErrorWithGoogleAuth,
                HttpStatusCode.BadRequest);
        }

        public static Exception UserNotFoundException(string userId)
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"User with id:{userId} not found"),
                        ErrorCodes.UserNotFound,
                        HttpStatusCode.NotFound);
        }

        public static Exception UserWithIdNotFoundException(string id)
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"User with id {id} not found"),
                        ErrorCodes.UserWithIdNotFound,
                        HttpStatusCode.NotFound);
        }

        public static Exception UserWithEmailAlreadyExistException(string email)
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"User with email {email} already exist"),
                        ErrorCodes.UserWithEmailAlreadyExist,
                        HttpStatusCode.NotFound);
        }
    }
}
