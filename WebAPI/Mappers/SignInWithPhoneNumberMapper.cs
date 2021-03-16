using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebAPI.Models;

namespace Mit_Oersted.WebAPI.Mappers
{
    public class SignInWithPhoneNumberMapper : IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto>
    {
        public TokenResponseBodyDto Map(SignInWithPhoneNumberResponseDto source)
        {
            return new TokenResponseBodyDto() {
                RefreshToken = source.RefreshToken,
                IdToken = source.IdToken
            };
        }
    }
}
