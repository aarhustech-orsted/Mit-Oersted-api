﻿using System.Text.Json.Serialization;

namespace Mit_Oersted.WebAPI.Models
{
    public class LogUserInWithPhoneBodyDto
    {
        [JsonInclude]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
