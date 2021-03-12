using System;

namespace Mit_Oersted.Domain.Events
{
    public class EventInfo
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string EventType { get; set; }
        public IEvent Event { get; set; }
    }
}
