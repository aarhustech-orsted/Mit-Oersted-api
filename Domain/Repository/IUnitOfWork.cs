namespace Mit_Oersted.Domain.Repository
{
    public interface IUnitOfWork
    {
        ITransactionRepository Transactions { get; }
        IUserRepository Users { get; }
        IAddressRepository Addresses { get; }
        IInvoiceRepository Invoices { get; }
    }
}
