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

        public static readonly string AddressNotFound = "AddressNotFound";
        public static readonly string AddressWithIdNotFound = "AddressWithIdNotFound";
        public static readonly string AddressAlreadyExist = "AddressAlreadyExist";

        public static readonly string InvoiceFolderNotFound = "InvoiceFolderNotFound";
        public static readonly string InvoiceFileNotFound = "InvoiceFileNotFound";
        public static readonly string InvoiceFileInFolderNotFound = "InvoiceFileInFolderNotFound";
        public static readonly string InvoiceWithIdNotFound = "InvoiceWithIdNotFound";
        public static readonly string InvoiceAlreadyExist = "InvoiceAlreadyExist";
    }
}
