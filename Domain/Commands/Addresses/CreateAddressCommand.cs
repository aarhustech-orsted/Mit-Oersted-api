namespace Mit_Oersted.Domain.Commands.Addresses
{
    public class CreateAddressCommand
    {
        public string UserId { get; set; }
        public string AddressString { get; set; }
    }
}
