using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataUsageModel
    {
        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("kwh")]
        public int Kwh { get; set; }
    }
}
