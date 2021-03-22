using System.Collections.Generic;

namespace Mit_Oersted.WebApi.Models.Invoices
{
    public class InvoiceDto
    {
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
    }
}
