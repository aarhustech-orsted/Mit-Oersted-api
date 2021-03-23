using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataPrimaryAddressModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
