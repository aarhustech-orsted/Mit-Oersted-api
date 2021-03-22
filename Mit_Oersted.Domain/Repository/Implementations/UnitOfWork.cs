using System;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            IUserRepository users,
            IAddressRepository addresses,
            IInvoiceRepository invoices
            )
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
            Addresses = addresses ?? throw new ArgumentNullException(nameof(addresses));
            Invoices = invoices ?? throw new ArgumentNullException(nameof(invoices));
        }

        public IUserRepository Users { get; private set; }
        public IAddressRepository Addresses { get; private set; }
        public IInvoiceRepository Invoices { get; private set; }
    }
}
