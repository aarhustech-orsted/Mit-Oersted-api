using Mit_Oersted.Domain.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository
{
    public interface IAddressRepository
    {
        Task<List<AddressModel>> GetAllAsync();
        Task<AddressModel> GetByIdAsync(string id);

        Task<string> AddAsync(AddressModel model);
        void RemoveAsync(AddressModel model);
        void UpdateAsync(string id, Dictionary<string, object> updates);

        public bool IsAddressAlreadyInUse(string id);
        public string Base64Encode(string plainText);
        public string Base64Decode(string base64EncodedData);
    }
}
