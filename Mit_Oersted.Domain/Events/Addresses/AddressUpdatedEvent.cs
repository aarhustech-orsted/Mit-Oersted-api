namespace Mit_Oersted.Domain.Events.Addresses
{
    internal class AddressUpdatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
