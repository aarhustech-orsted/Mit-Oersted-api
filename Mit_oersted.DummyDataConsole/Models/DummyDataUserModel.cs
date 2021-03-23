using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataUserModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("primary_address")]
        public DummyDataPrimaryAddressModel PrimaryAddress { get; set; }

        [JsonProperty("addresses")]
        public DummyDataAddressModel[] Addresses { get; set; }
    }
}
