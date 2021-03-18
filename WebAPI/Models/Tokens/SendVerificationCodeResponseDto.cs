using System.Text.Json.Serialization;

namespace Mit_Oersted.WebApi.Models.Tokens
{
    public class SendVerificationCodeResponseDto
    {
        [JsonInclude]
        [JsonPropertyName("sessionInfo")]
        public string SessionInfo { get; set; }
    }
}
