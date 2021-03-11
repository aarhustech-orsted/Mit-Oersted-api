using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Domain.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(
            ITransactionRepository transactions,
            ILogger<UnitOfWork> logger
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        }

        public async Task SaveAsync()
        {
            try
            {
                var count = 0;
                _logger.LogDebug($"{count} state entries written to database");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when saving to database", ex);
                throw;
            }
        }

        public void Save()
        {
            try
            {
                var count = 0;
                _logger.LogDebug($"{count} state entries written to database");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when saving to database", ex);
                throw;
            }
        }

        public ITransactionRepository Transactions { get; private set; }
    }
}
