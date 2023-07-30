using Project1.Models;

namespace Project1
{
    public class Processor
    {
        private Database.DatabaseWorker _databaseWorker;
        private List<Variable> _variables;
        private List<Event> _events;
        private List<EventHistory> _eventsHistories;
        private Dictionary<Variable, DateTime> _lastTrending;

        public Processor() 
        {
            _databaseWorker = new Database.DatabaseWorker("");
            _init();
            while(true)
            {
                _process();
                Thread.Sleep(50);
            }
        }

        public List<Variable>? GetVariables(int[]? ids=null) => ids==null? _variables: ids.Select(id => _variables.FirstOrDefault(_ => _.Id == id)).ToList();

        public List<EventHistory>? GetEventHistories(int? variableId = null, DateTime? start = null, DateTime? end = null, EventType? type = null, int? count = 0)
        {
            Variable? variable = variableId == null
                ?null
                : _variables.FirstOrDefault(_ => _.Id == variableId); 

            return _databaseWorker.ReadEventsHistory(_events, variable, start, end, type, count);
        }

        public List<Trend>? GetTrend(List<Variable> variables, DateTime? start, DateTime? end)=> _databaseWorker.ReadTrends(variables,start,end);
        public List<Event>? GetEvents(List<Variable> variables) => _databaseWorker.ReadEvents(variables);

        public void ChangeVariables(List<Variable> variables)
        {
            foreach(var variable in variables) 
            {
                _variables.FirstOrDefault(_ => _.Id == variable.Id).Value = variable.Value;
            }
            //WriteToPLC;
        }

        private void _init()
        {
            _variables = _databaseWorker.ReadVariables();
            _events = _databaseWorker.ReadEvents(_variables);
            _eventsHistories = _databaseWorker.ReadOpenedEventsHistory(_events);
            _lastTrending = new Dictionary<Variable, DateTime>();
            var now = DateTime.Now;

            foreach(var variable in _variables)
            {
                if (variable.TrendingPeriod>0)
                {
                    _lastTrending.Add(variable, now);
                }
            }
        }

        private void _process()
        {
            _processEvents();
            _processTrends();
        }

        private void _processTrends()
        {
            var now = DateTime.Now;
            var toTrend = new List<Variable>();

            foreach (var lastTrend in _lastTrending)
            {
                if ((now-lastTrend.Value).TotalMicroseconds>lastTrend.Key.TrendingPeriod)
                {
                    toTrend.Add(lastTrend.Key);
                }
            }

            _databaseWorker.WriteTrendValue(toTrend);        
        }

        private void _processEvents()
        {
            foreach (var ev in _events)
            {
                var eventFired = ev.Check();
                var eventHistories = _eventsHistories.Where(_ => _.Event == ev);

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

                    _eventsHistories.Add(eventHistory);
                    _databaseWorker.WriteEventsHistory(eventHistory);
                    continue;
                }

                if (!eventFired &&
                    eventHistories != null
                    && eventHistories.Count()>=0)
                {
                    foreach (var eventHistory in eventHistories)
                    {
                        if (eventHistory != null)
                        {
                            eventHistory.EndTime = DateTime.Now;
                            _databaseWorker.WriteEndTimeEventHistory(eventHistory);
                            _eventsHistories.Remove(eventHistory);
                        }
                    }
                }
            }
        }
    }
}
