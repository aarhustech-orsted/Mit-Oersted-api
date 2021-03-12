namespace Mit_Oersted.Domain.Events.User
{
    internal class UserUpdatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
