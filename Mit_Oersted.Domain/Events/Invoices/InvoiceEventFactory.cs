namespace Mit_Oersted.Domain.Events.Invoices
{
    public class InvoiceEventFactory
    {
        public IEvent GetUserCreatedEvent(string id)
        {
            return new InvoiceCreatedEvent()
            {
                Id = id
            };
        }

        public IEvent GetUserUpdatedEvent(string id)
        {
            return new InvoiceUpdatedEvent()
            {
                Id = id
            };
        }
    }
}
