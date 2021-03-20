namespace Mit_Oersted.Domain.Events.Users
{
    internal class UserUpdatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
