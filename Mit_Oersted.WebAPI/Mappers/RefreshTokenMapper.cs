using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebApi.Models.Tokens;

namespace Mit_Oersted.WebApi.Mappers
{
    public class RefreshTokenMapper : IMapper<RefreshTokenResponseDto, TokenResponseBodyDto>
    {
        public TokenResponseBodyDto Map(RefreshTokenResponseDto source)
        {
            return new TokenResponseBodyDto()
            {
                RefreshToken = source.RefreshToken,
                IdToken = source.IdToken
            };
        }
    }
}
