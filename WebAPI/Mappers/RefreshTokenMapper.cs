using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebAPI.Models;

namespace Mit_Oersted.WebAPI.Mappers
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
