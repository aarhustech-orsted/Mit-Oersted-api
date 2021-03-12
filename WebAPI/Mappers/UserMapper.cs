using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebAPI.Models;

namespace Mit_Oersted.WebAPI.Mappers
{
    public class UserMapper : IMapper<User, UserDto>
    {
        public UserDto Map(User source)
        {
            return new UserDto
            {
                Id = source.Id,
                Email = source.Email
            };
        }
    }
}
