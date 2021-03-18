namespace Mit_Oersted.Domain.Events.Invoices
{
    internal class InvoiceUpdatedEvent : IEvent
    {
        public string Id { get; set; }
    }
}
