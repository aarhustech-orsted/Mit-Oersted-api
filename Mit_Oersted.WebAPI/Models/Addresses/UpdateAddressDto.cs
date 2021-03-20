namespace Mit_Oersted.WebApi.Models.Addresses
{
    public class UpdateAddressDto
    {
        public string UserId { get; set; }
        public string AddressString { get; set; }
        public string InvoiceFolder { get; set; }
    }
}
