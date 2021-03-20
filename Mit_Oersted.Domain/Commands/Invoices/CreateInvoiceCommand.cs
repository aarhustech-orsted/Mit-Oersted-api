using System.Collections.Generic;

namespace Mit_Oersted.Domain.Commands.Invoices
{
    public class CreateInvoiceCommand
    {
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public byte[] File { get; set; }
    }
}
