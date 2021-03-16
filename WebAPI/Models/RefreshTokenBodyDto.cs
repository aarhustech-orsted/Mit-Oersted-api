using System.Text.Json.Serialization;

namespace Mit_Oersted.WebAPI.Models
{
    public class RefreshTokenBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "refresh_token";

        [JsonInclude]
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
