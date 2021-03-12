using System.Text.Json.Serialization;

namespace Mit_Oersted.Models
{
    public class SignInWithPhoneNumberBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("sessionInfo")]
        public string SessionInfo { get; set; }

        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonInclude]
        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }
    }
}
