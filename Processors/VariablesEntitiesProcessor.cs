using Project1.Database;
using Project1.Models;
using System.Linq;

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

        public List<Variable> SetVariableValueByEntity(List<VariableEntity> variablesValues) =>
            SetVariableValueByEntity(variablesValues.ToDictionary(_ => _, _ => _.Variable.Value));

        public List<Variable> SetVariableValueByEntity(Dictionary<int, string> variablesValues) =>
            SetVariableValueByEntity(
                variablesValues.ToDictionary(_ => _variableEntities.FirstOrDefault(e => e.Id == _.Key), _=>_.Value)
                );

        public List<Variable> SetVariableValueByEntity(Dictionary<VariableEntity, string> variablesValues) =>
            SetVariableValueByEntity(
                variablesValues.ToDictionary(
                    _ => _variableEntities.FirstOrDefault(e => e.Id == _.Key.Id), 
                    _ => ValuesParsing.GetValueFromString(_.Value, _.Key.Variable.Type)
                    )
                );

        public List<Variable> SetVariables(Dictionary<int, string> variablesValues) =>
            SetVariables(
                variablesValues.ToDictionary(
                    key => _variableEntities.FirstOrDefault(_ => _.Variable.Id == key.Key).Variable, 
                    _ => _.Value
                    )
                );

        public List<Variable> SetVariables(Dictionary<Variable, string> variablesValues) => 
            SetVariables(
                variablesValues.ToDictionary(_ => _.Key, _ => ValuesParsing.GetValueFromString(_.Value,_.Key.Type))
                );

        public List<Variable> SetVariables(Dictionary<int, object> variablesValues) => 
            SetVariables(
                variablesValues.ToDictionary(
                    key => _variableEntities.FirstOrDefault(_=>_.Variable.Id == key.Key).Variable, 
                    _ => _.Value
                    )
                );

        public List<int> WriteableVariablesIds => _variableEntities
            .Where(_ => _.Writable && _.Variable.Id != null)
            .Select(_=>(int)_.Variable.Id)
            .Distinct()
            .ToList();

        public List<Variable> SetVariables(List<Variable> variables) =>
            SetVariables(
                variables
                    .Where(variable=>variable!= null && variable.Id != null)
                    .ToDictionary(variable => (int)variable.Id, variable => variable.Value)
                );

        public List<Variable> SetVariableValueByEntity(Dictionary<VariableEntity,object> variables)=>
            SetVariables(
                variables
                    .Where(_ => _.Key.Writable && _.Key.Variable.Id != null)
                    .ToDictionary(e=>e.Key.Variable,e=>e.Value)
                );

        public List<Variable> SetVariables(Dictionary<Variable, object> variables)
        {
            foreach (var variable in variables)
            {
                variable.Key.Value = variable.Value;
            }

            return variables.Keys.ToList();
        }

    }
}
