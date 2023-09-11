using Project1.Database;
using Project1.Models;

namespace Project1.Processors
{
    public class TrendsProcessor
    {
        private readonly DatabaseWorker _databaseWorker;
        private Dictionary<Variable, DateTime> _lastTrending;

        public TrendsProcessor(
            DatabaseWorker databaseWorker,
            List<Variable> variables) 
        { 
            _databaseWorker= databaseWorker;

            _lastTrending = new Dictionary<Variable, DateTime>();
            var now = DateTime.Now;

            if (variables == null)
                return;

            foreach (var variable in variables)
            {
                if (variable.TrendingPeriod > 0)
                {
                    _lastTrending.Add(variable, now);
                }
            }
        }

        public async Task<List<Trend>> ReadTrends(List<Variable> variables, DateTime? startTime, DateTime? endTime) => 
            await _databaseWorker.ReadTrends(variables, startTime, endTime);

        public async Task Process()
        {
            var now = DateTime.Now;
            var toTrend = new List<Variable>();

            if (_lastTrending == null)
                return;

            foreach (var lastTrend in _lastTrending)
            {
                if ((now - lastTrend.Value).TotalMicroseconds > lastTrend.Key.TrendingPeriod)
                {
                    toTrend.Add(lastTrend.Key);
                }
            }

            await _databaseWorker.WriteTrendValue(toTrend);
        }

    }
}
