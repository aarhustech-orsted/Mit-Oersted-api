namespace Mit_Oersted.Domain.Events.Invoices
{
    internal class InvoiceCreatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
