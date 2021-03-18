using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository
{
    public interface ITransactionRepository
    {
        void Add(TransactionModel entity);
        PaginationResult<TransactionModel> GetByFilter(PaginationQuery paginationQuery);
        Task<IEnumerable<TransactionModel>> GetAllAsync();
    }
}
