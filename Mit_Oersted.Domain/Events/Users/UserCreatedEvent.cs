namespace Mit_Oersted.Domain.Events.Users
{
    internal class UserCreatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
