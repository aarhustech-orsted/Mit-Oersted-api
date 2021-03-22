namespace Mit_Oersted.Domain.Repository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IAddressRepository Addresses { get; }
        IInvoiceRepository Invoices { get; }
    }
}
