using Mit_Oersted.Domain.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(string userId);
        Task<User> GetByEmailAsync(string userEmail);

        Task<string> Add(User userBbModel);
        void Remove(User userBbModel);
        void Update(string userId, Dictionary<string, object> updates);

        bool IsEmailAlreadyInUse(string userEmail);
    }
}
