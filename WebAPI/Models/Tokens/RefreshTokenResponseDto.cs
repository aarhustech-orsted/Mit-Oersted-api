using System.Text.Json.Serialization;

namespace Mit_Oersted.WebApi.Models.Tokens
{
    public class RefreshTokenResponseDto
    {
        [JsonInclude]
        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonInclude]
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonInclude]
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonInclude]
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonInclude]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonInclude]
        [JsonPropertyName("project_id")]
        public string ProjectId { get; set; }
    }
}
