using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.WebApi.Models.Invoices;

namespace Mit_Oersted.WebApi.Mappers
{
    public class InvoiceMapper : IMapper<InvoiceModel, InvoiceDto>
    {
        public InvoiceDto Map(InvoiceModel source)
        {
            return new InvoiceDto
            {
                FileName = source.Name,
                DownloadUrl = source.DownloadUrl
            };
        }
    }
}
