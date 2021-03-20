using System.Text.Json.Serialization;

namespace Mit_Oersted.WebApi.Models.Tokens
{
    public class TokenResponseBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonInclude]
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }
}
