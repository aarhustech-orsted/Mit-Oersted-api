using Microsoft.Extensions.Logging;
using System;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(
            ITransactionRepository transactions,
            IUserRepository users,
            IAddressRepository addresses,
            IInvoiceRepository invoices,
            ILogger<UnitOfWork> logger
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
            Users = users ?? throw new ArgumentNullException(nameof(users));
            Addresses = addresses ?? throw new ArgumentNullException(nameof(addresses));
            Invoices = invoices ?? throw new ArgumentNullException(nameof(invoices));
        }

        public ITransactionRepository Transactions { get; private set; }
        public IUserRepository Users { get; private set; }
        public IAddressRepository Addresses { get; private set; }
        public IInvoiceRepository Invoices { get; private set; }
    }
}
