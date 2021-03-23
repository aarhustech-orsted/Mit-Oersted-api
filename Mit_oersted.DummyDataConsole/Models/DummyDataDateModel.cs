using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataDateModel
    {
        [JsonProperty("day")]
        public int Day { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
