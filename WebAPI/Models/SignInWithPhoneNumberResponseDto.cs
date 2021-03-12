using System.Text.Json.Serialization;

namespace Mit_Oersted.Models
{
    public class SignInWithPhoneNumberResponseDto
    {
        [JsonInclude]
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
        [JsonInclude]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonInclude]
        [JsonPropertyName("expiresIn")]
        public string ExpiresIn { get; set; }
        [JsonInclude]
        [JsonPropertyName("localId")]
        public string LocalId { get; set; }
        [JsonInclude]
        [JsonPropertyName("isNewUser")]
        public bool IsNewUser { get; set; }
        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
