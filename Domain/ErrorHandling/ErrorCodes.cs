namespace Mit_Oersted.Domain.ErrorHandling
{
    public static class ErrorCodes
    {
        public static readonly string MappingOfRecipeFailed = "MappingCommandFailed";
        public static readonly string CommandIsNull = "CommandIsNull";
        public static readonly string ErrorWithGoogleAuth = "ErrorWithGoogleAuth";
        public static readonly string UserNotFound = "UserNotFound";
        public static readonly string UserWithIdNotFound = "UserWithIdNotFound";
        public static readonly string UserWithEmailAlreadyExist = "UserWithEmailAlreadyExist";
    }
}
