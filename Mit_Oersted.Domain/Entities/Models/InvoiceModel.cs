using System.Collections.Generic;

namespace Mit_Oersted.Domain.Entities.Models
{

    //TODO: Add byte data as well?
    public class InvoiceModel
    {
        public string Bucket { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public string DownloadUrl { get; set; }
    }
}
