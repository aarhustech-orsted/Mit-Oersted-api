using Mit_Oersted.Domain.Entities.Models;

namespace Mit_Oersted.Domain.Events.Users
{
    public class UserEventFactory
    {
        public IEvent GetUserCreatedEvent(UserModel model)
        {
            return new UserCreatedEvent()
            {
                Id = model.Id
            };
        }

        public IEvent GetUserUpdatedEvent(UserModel model)
        {
            return new UserUpdatedEvent()
            {
                Id = model.Id
            };
        }
    }
}
