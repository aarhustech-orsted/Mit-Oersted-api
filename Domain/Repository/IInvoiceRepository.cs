using Mit_Oersted.Domain.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository
{
    public interface IInvoiceRepository
    {
        Task<List<InvoiceModel>> GetAllAsync();
        Task<List<InvoiceModel>> GetFolderByIdAsync(string id);
        Task<InvoiceModel> GetFileByIdAsync(string id);

        Task<string> AddAsync(string id, Dictionary<string, string> metadata, byte[] fileData);
        void RemoveAsync(string id);
        void UpdateAsync(string id, Dictionary<string, string> updates);

        public bool IsInvoiceAlreadyInUse(string id);
    }
}
