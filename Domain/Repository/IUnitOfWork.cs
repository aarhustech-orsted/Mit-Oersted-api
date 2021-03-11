using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
        ITransactionRepository Transactions { get; }
    }
}
