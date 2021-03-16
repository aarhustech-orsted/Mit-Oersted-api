using System.Text.Json.Serialization;

namespace Mit_Oersted.WebAPI.Models
{
    public class SendVerificationCodeResponseDto
    {
        [JsonInclude]
        [JsonPropertyName("sessionInfo")]
        public string SessionInfo { get; set; }
    }
}
