using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebApi.Models.Addresses;

namespace Mit_Oersted.WebApi.Mappers
{
    public class AddressMapper : IMapper<AddressModel, AddressDto>
    {
        public AddressDto Map(AddressModel source)
        {
            return new AddressDto
            {
                Id = source.Id,
                AddressString = source.AddressString,
                UserId = source.UserId
            };
        }
    }
}
