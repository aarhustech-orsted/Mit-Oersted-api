using System.Text.Json.Serialization;

namespace Mit_Oersted.Models
{
    public class SendVerificationCodeBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
