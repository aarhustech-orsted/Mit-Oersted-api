using Newtonsoft.Json;

namespace Mit_Oersted.DummyDataConsole.Models
{
    public class DummyDataAddressModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("measure_device")]
        public string MeasureDevice { get; set; }

        [JsonProperty("invoices")]
        public DummyDataInvoiceModel[] Invoices { get; set; }
    }
}
