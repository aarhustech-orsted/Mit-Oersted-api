namespace Mit_Oersted.Domain.Commands.Addresses
{
    public class UpdateAddressCommand
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AddressString { get; set; }
    }
}
