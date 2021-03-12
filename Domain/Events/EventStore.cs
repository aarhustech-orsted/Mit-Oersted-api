using FoodieCommunityCase.Domain.Events.Serialization;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.Domain.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mit_Oersted.Domain.Events
{
    public class EventStore : IEventStore
    {
        private readonly JsonSerializerExt _serializer;
        private readonly ITimeService _timeService;
        private readonly IUnitOfWork _unitOfWork;

        public EventStore(
            ITimeService timeService,
            IUnitOfWork unitOfWork
            )
        {
            _serializer = new JsonSerializerExt();
            _timeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public void AddEvents(params IEvent[] events)
        {
            AddEvents(events.AsEnumerable());
        }

        public void AddEvents(IEnumerable<IEvent> events)
        {
            foreach (IEvent e in events)
            {
                _unitOfWork.Transactions.Add(CreateTransactionFromEvent(e, _serializer, _timeService.UtcNow));
            }
        }

        public static Transaction CreateTransactionFromEvent(IEvent e, JsonSerializerExt serializer, DateTime now)
        {
            return new Transaction()
            {
                Created = now,
                EventType = e.GetType().Name,
                EventData = serializer.Serialize(e)
            };
        }

        public IEnumerable<EventInfo> GetEvents()
        {
            var result = new List<EventInfo>();

            foreach (var item in _unitOfWork.Transactions.GetAllAsync().Result)
            {
                result.Add(new EventInfo
                {
                    Id = item.Id,
                    Created = item.Created,
                    EventType = item.EventType,
                    Event = (IEvent)_serializer.Deserialize(item.EventData)
                });
            }

            return result;
        }
    }
}
