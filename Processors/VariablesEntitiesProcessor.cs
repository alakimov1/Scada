using Project1.Database;
using Project1.Models;
using System.Linq;
using Project1.Controllers.VariablesController;

namespace Project1.Processors
{
    public class VariablesEntitiesProcessor
    {
        private readonly DatabaseWorker _databaseWorker;
        private List<VariableEntity>? _variableEntities;

        public VariablesEntitiesProcessor(
            DatabaseWorker databaseWorker)
        {
            _databaseWorker = databaseWorker;
        }

        public async Task Init()
        {
            _variableEntities = await _databaseWorker.ReadVariablesEntities();
        }

        public List<Subgroup> GetSubgroups => _variableEntities.Select(_ => _.Subgroup).Distinct().ToList();

        public List<Group> GetGroups => 
            _variableEntities.Select(_ => _.Subgroup).Distinct().Select(_=>_.Group).Distinct().ToList();

        public List<VariableEntity> GetVariablesEntities => _variableEntities;

        public List<VariableEntity> GetVariablesEntitiesBySubgroup(int id) => 
            _variableEntities.Where(_ => _.Subgroup?.Id == id).ToList();
        public List<VariableEntity> GetVariablesEntitiesBySubgroups(List<int> ids) => 
            _variableEntities
                .Where(_ => _.Subgroup != null && _.Subgroup?.Id != null && ids.Contains((int)_.Subgroup?.Id))
                .ToList();

        public List<Subgroup> GetSubgroupsByGroup(int id) => GetSubgroups.Where(_ => _.Group.Id == id).ToList();

        public List<VariableEntity> GetVariableEntitiesByGroup(int id) => 
            _variableEntities.Where(_ => _.Subgroup.Group.Id == id).ToList();
        
        public List<VariableEntity> GetVariableEntitiesByGroups(List<int> ids) => 
            _variableEntities
                .Where(_ => _.Subgroup?.Group?.Id != null && ids.Contains((int)_.Subgroup?.Group?.Id))
                .ToList();

        public List<Variable> GetVariables(int[]? ids = null)
        {
            if (_variableEntities == null)
                return null;

            return ids == null || ids.Count() == 0
            ? _variableEntities.Select(_ => _.Variable).Distinct().ToList()
            : ids.Select(id => _variableEntities.FirstOrDefault(_ => _.Variable.Id == id).Variable).ToList();
        }

        public Dictionary<int, VariableValueValidationResult> SetVariables(Dictionary<Variable, object> variables)
        {
            var dictToReturn = new Dictionary<int, VariableValueValidationResult>();

            foreach (var variable in variables)
            {
                var success = ValuesParsing.TryParse(variable.Value as string, variable.Key.Type, out var result);

                if (success)
                {
                    variable.Key.Value = result;
                    dictToReturn.Add((int)variable.Key.Id, VariableValueValidationResult.Ok);
                }
                else
                {
                    dictToReturn.Add((int)variable.Key.Id, VariableValueValidationResult.IncorrectValue);
                }
            }

            return dictToReturn;
        }

    }
}
