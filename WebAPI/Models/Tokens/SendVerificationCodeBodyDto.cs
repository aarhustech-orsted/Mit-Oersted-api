using System.Text.Json.Serialization;

namespace Mit_Oersted.WebApi.Models.Tokens
{
    public class SendVerificationCodeBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
