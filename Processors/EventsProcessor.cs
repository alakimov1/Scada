using Project1.Database;
using Project1.Models;

namespace Project1.Processors
{
    public class EventsProcessor
    {
        private readonly DatabaseWorker _databaseWorker;
        private List<EventType>? _eventTypes;
        private List<Event>? _events;
        private List<EventHistory>? _eventsHistories;
        private List<Variable>? _variables;

        public EventsProcessor(
            DatabaseWorker databaseWorker)
        {
            _databaseWorker = databaseWorker;
        }

        public async Task Init(List<Variable> variables)
        {
            _variables = variables;

            if (variables == null)
                return;

            _eventTypes = await _databaseWorker.DatabaseWorkerEvents.ReadEventTypes();
            _events = await _databaseWorker.DatabaseWorkerEvents.ReadEvents(variables, _eventTypes);
            _eventsHistories = await _databaseWorker.DatabaseWorkerEvents.ReadOpenedEventsHistory(_events);
        }

        public async Task<List<Event>?> GetEvents(List<Variable> variables) => 
            await _databaseWorker.DatabaseWorkerEvents.ReadEvents(variables, _eventTypes!);
        
        public async Task<List<EventType>?> GetEventTypes() => 
            await _databaseWorker.DatabaseWorkerEvents.ReadEventTypes();
        
        public async Task<List<EventHistory>?> GetEventHistories(
            int? variableId = null, 
            DateTime? start = null, 
            DateTime? end = null, 
            List<int>? type = null, 
            int? count = 0)
        {
            Variable? variable = variableId == null
                ? null
                : _variables?.FirstOrDefault(_ => _.Id == variableId);

            return await _databaseWorker
                            .DatabaseWorkerEvents
                            .ReadEventsHistory(_events!, variable, start, end, type, count);
        }

        public async Task Process()
        {
            if (_events == null
                || _eventsHistories == null)
                return;

            foreach (var ev in _events)
            {
                var eventFired = ev.Check();
                var eventHistories = _eventsHistories?.Where(_ => _.Event.Id == ev.Id);

                if (eventFired
                    && (eventHistories == null
                    || eventHistories.Count() <= 0))
                {
                    var eventHistory = new EventHistory()
                    {
                        Id = null,
                        Event = ev,
                        StartTime = DateTime.Now,
                        EndTime = new DateTime(0)
                    };

                    _eventsHistories!.Add(eventHistory);
                    await _databaseWorker.DatabaseWorkerEvents.WriteEventsHistory(eventHistory);
                    continue;
                }

                if (!eventFired &&
                    eventHistories != null
                    && eventHistories.Count() >= 0)
                {
                    foreach (var eventHistory in eventHistories)
                    {
                        if (eventHistory != null)
                        {
                            eventHistory.EndTime = DateTime.Now;
                            await _databaseWorker.DatabaseWorkerEvents.WriteEndTimeEventHistory(eventHistory);
                            _eventsHistories!.Remove(eventHistory);
                        }
                    }
                }
            }
        }
    }
}
