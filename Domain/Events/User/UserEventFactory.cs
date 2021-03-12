namespace Mit_Oersted.Domain.Events.User
{
    public class UserEventFactory
    {
        public IEvent GetUserCreatedEvent(Entities.Models.User user)
        {
            return new UserCreatedEvent()
            {
                Id = user.Id
            };
        }

        public IEvent GetUserUpdatedEvent(Entities.Models.User user)
        {
            return new UserUpdatedEvent()
            {
                Id = user.Id
            };
        }
    }
}
