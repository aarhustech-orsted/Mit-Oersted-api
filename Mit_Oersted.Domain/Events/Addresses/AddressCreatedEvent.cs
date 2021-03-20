namespace Mit_Oersted.Domain.Events.Addresses
{
    internal class AddressCreatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
