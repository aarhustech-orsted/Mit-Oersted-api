using Mit_Oersted.Domain.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository
{
    public interface IUserRepository
    {
        Task<List<UserModel>> GetAllAsync();
        Task<UserModel> GetByIdAsync(string id);
        Task<UserModel> GetByEmailAsync(string id);

        Task<string> AddAsync(UserModel model);
        void RemoveAsync(UserModel model);
        void UpdateAsync(string id, Dictionary<string, object> updates);

        bool IsEmailAlreadyInUse(string id);
    }
}
