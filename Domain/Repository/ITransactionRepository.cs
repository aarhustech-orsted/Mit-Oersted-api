using Domain.Entities.Models;
using Domain.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface ITransactionRepository
    {
        void Add(Transaction entity);
        PaginationResult<Transaction> GetByFilter(PaginationQuery paginationQuery);
        Task<IEnumerable<Transaction>> GetAllAsync();
    }
}
