using Project1.Database;
using Project1.Models;

namespace Project1.Processors
{
    public class Processor
    {
        public TrendsProcessor TrendsProcessor;
        public EventsProcessor EventsProcessor;
        private DatabaseWorker _databaseWorker;
        private List<Variable>? _variables;
        public static Processor? Instance;

        public async static Task Init()
        {
            Instance = new Processor();
            await Instance.InitInstance();
        }

        private async Task InitInstance()
        {
            _databaseWorker = new DatabaseWorker("Data Source=C:\\Users\\1\\source\\repos\\Project1\\Database\\DB.db");
            _variables = await _databaseWorker.ReadVariables();
            TrendsProcessor = new TrendsProcessor(_databaseWorker, _variables);
            EventsProcessor = new EventsProcessor(_databaseWorker);
            await EventsProcessor.Init(_variables);
        }

        public async Task Process()
        {
            await EventsProcessor.Process();
            await TrendsProcessor.Process();
        }

        public List<Variable>? GetVariables(int[]? ids = null) =>
            ids == null || ids.Count() == 0
            ? _variables
            : ids.Select(id => _variables.FirstOrDefault(_ => _.Id == id)).ToList();

        public void ChangeVariables(List<Variable> variables)
        {
            if (_variables == null)
                return;

            foreach (var variable in variables)
            {
                _variables.FirstOrDefault(_ => _.Id == variable.Id).Value = variable.Value;
            }
            //WriteToPLC;
        }

        
    }
}
