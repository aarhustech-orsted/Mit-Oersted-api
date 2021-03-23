using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataInvoiceModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("date")]
        public DummyDataDateModel Date { get; set; }

        [JsonProperty("usage")]
        public DummyDataUsageModel Usage { get; set; }

        [JsonProperty("measure")]
        public int Measure { get; set; }
    }
}
