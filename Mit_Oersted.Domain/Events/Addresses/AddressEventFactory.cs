using Mit_Oersted.Domain.Entities.Models;

namespace Mit_Oersted.Domain.Events.Addresses
{
    public class AddressEventFactory
    {
        public IEvent GetAddressCreatedEvent(AddressModel model)
        {
            return new AddressCreatedEvent()
            {
                Id = model.Id
            };
        }

        public IEvent GetAddressUpdatedEvent(AddressModel model)
        {
            return new AddressUpdatedEvent()
            {
                Id = model.Id
            };
        }
    }
}
