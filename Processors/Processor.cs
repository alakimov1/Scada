using Project1.Database;
using Project1.Models;

namespace Project1.Processors
{
    public class Processor
    {
        private DatabaseWorker? _databaseWorker;
        private List<Variable> _variables;
        public static Processor? Instance;
        public TrendsProcessor? TrendsProcessor { get; private set; }
        public EventsProcessor? EventsProcessor { get; private set; }
        public VariablesEntitiesProcessor? VariablesEntitiesProcessor { get; private set; }

        private static bool _initializationInProcess = false;

        public async static Task Init()
        {
            if (Instance == null)
            {
                Instance = new Processor();
                await Instance.InitInstance();
            }
        }

        private async Task InitInstance()
        {
            _databaseWorker = new DatabaseWorker("Data Source=C:\\Users\\1\\source\\repos\\Project1\\Database\\DB.db");
            VariablesEntitiesProcessor = new VariablesEntitiesProcessor(_databaseWorker);
            VariablesEntitiesProcessor.Init();
            _variables = VariablesEntitiesProcessor.GetVariables();
            TrendsProcessor = new TrendsProcessor(_databaseWorker, _variables);
            EventsProcessor = new EventsProcessor(_databaseWorker);
            await EventsProcessor.Init(_variables);
        }

        public static async Task Process()
        {
            if (_initializationInProcess)
                return;

            if (Instance == null)
            {
                _initializationInProcess = true;
                await Processor.Init();
                _initializationInProcess = false;
            }
            else
            {
                await Instance.ProcessVariables();
            }
        }

        private async Task ProcessVariables()
        {
            await ReadVariablesFromPLC(_variables);
            await EventsProcessor.Process();
            await TrendsProcessor.Process();
        }

        public async Task ChangeVariables(List<VariableEntity> variables)
        {
            var variablesChanged = VariablesEntitiesProcessor.SetVariableValueByEntity(variables);
            await WriteVariablesToPLC(variablesChanged);
        }

        private async Task ReadVariablesFromPLC(List<Variable> variables)
        {

        }

        private async Task WriteVariablesToPLC(List<Variable> variables)
        {

        }
    }
}
