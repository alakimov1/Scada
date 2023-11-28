using Project1.Database;
using Project1.Models;
using Project1.Controllers.VariablesController;
using Project1.Services.Analytics;

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
            await VariablesEntitiesProcessor.Init();
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
                await Init();
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

        public async Task<Dictionary<int, VariableValueValidationResult>> ChangeVariables(VariableChangeQuery[] variablesToChange)
        {
            var variablesToSetDict = new Dictionary<Variable, object>();

            foreach(var variableToChange in variablesToChange)
            {
                var variable = _variables.FirstOrDefault(variable => variable.Id == variableToChange.Id);

                if (variable != null)
                    variablesToSetDict.Add(variable, variableToChange.Value);
            }

            var variablesChanged = VariablesEntitiesProcessor.SetVariables(variablesToSetDict);
            await WriteVariablesToPLC(
                variablesToSetDict
                    .Where(_ => variablesChanged.ContainsKey((int)_.Key.Id) && variablesChanged[(int)_.Key.Id] == VariableValueValidationResult.Ok)
                    .Select(_ => _.Key)
                    .ToList());

            foreach(var variableToSet in variablesToSetDict)
                Analytics.Write($"Пользователь меняет значение переменной { variableToSet.Key.Name} на {variableToSet.Value.ToString()}");

            return variablesChanged;
        }

        public (int Id, object Value)[] GetVariablesValues(int[]? ids = null)
        {
            if (_variables == null || ids == null || ids.Count() == 0)
                return null;

            var idsToReturn = ids.Where(id => _variables.Any(_ => _.Id == id && _.Value != null));

            return idsToReturn
                .Select(id => (id, _variables.First(variable => variable.Id == id).Value))
                .ToArray();
        }

        private async Task ReadVariablesFromPLC(List<Variable> variables)
        {

        }

        private async Task WriteVariablesToPLC(List<Variable> variables)
        {

        }
    }
}
