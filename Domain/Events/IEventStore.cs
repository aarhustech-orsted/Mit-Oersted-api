using System.Collections.Generic;
namespace Mit_Oersted.Domain.Events
{
    public interface IEventStore
    {
        void AddEvents(params IEvent[] events);
        void AddEvents(IEnumerable<IEvent> events);

        IEnumerable<EventInfo> GetEvents();
    }
}
