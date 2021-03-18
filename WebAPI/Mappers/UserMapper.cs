using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebApi.Models.Users;

namespace Mit_Oersted.WebApi.Mappers
{
    public class UserMapper : IMapper<UserModel, UserDto>
    {
        public UserDto Map(UserModel source)
        {
            return new UserDto
            {
                Id = source.Id,
                Email = source.Email
            };
        }
    }
}
