using System.Text.Json.Serialization;

namespace Mit_Oersted.WebAPI.Models
{
    public class SendVerificationCodeBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
