namespace Mit_Oersted.Domain.Events.User
{
    internal class UserCreatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
