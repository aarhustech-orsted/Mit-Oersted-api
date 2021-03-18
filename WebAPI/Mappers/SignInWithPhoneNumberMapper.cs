using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebApi.Models.Tokens;

namespace Mit_Oersted.WebApi.Mappers
{
    public class SignInWithPhoneNumberMapper : IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto>
    {
        public TokenResponseBodyDto Map(SignInWithPhoneNumberResponseDto source)
        {
            return new TokenResponseBodyDto()
            {
                RefreshToken = source.RefreshToken,
                IdToken = source.IdToken
            };
        }
    }
}
