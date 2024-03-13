using Project1.Database;
using Project1.Models;

namespace Project1.Processors
{
    public class VariableValuesSubstitutor
    {
        private readonly DatabaseWorker _databaseWorker;
        private List<VariableValueDictionary>? _substitutions;

        public VariableValuesSubstitutor(
            DatabaseWorker databaseWorker)
        {
            _databaseWorker = databaseWorker;
        }

        public async Task Init(List<Variable> variables)
        {
            if (variables == null)
                return;

            _substitutions = await _databaseWorker.ReadVariableValueDictionaries(variables);
        }

        public object Substitute(int variableId, object valueToSubstitute)
        {
            if (_substitutions == null || _substitutions.Count == 0)
                return valueToSubstitute;

            var variableValueDictionary = _substitutions.FirstOrDefault(_ => _.Variable.Id == variableId);

            if (variableValueDictionary == null 
                || variableValueDictionary.Dictionary == null)
                return valueToSubstitute;

            var keyValuePair = variableValueDictionary.Dictionary.FirstOrDefault(
                _ => _.Key.Equals(valueToSubstitute));

            if (variableValueDictionary.Dictionary.Any(_ => _.Key.Equals(valueToSubstitute)))
                return valueToSubstitute;

            return variableValueDictionary.Dictionary.FirstOrDefault(
                _ => _.Key.Equals(valueToSubstitute)).Value;
        }
        
        public object ReverseSubstitute(int variableId, object valueToReverseSubstitute)
        {
            if (_substitutions == null)
                return valueToReverseSubstitute;

            var variableValueDictionary = _substitutions.FirstOrDefault(_ => _.Variable.Id == variableId);

            if (variableValueDictionary == null
                || variableValueDictionary.Dictionary == null)
                return valueToReverseSubstitute;

            var keyValuePair = variableValueDictionary.Dictionary.FirstOrDefault(
                _ => _.Value.Equals(valueToReverseSubstitute));

            if (variableValueDictionary.Dictionary.Any(_ => _.Value.Equals(valueToReverseSubstitute)))
                return valueToReverseSubstitute;

            return variableValueDictionary.Dictionary.FirstOrDefault(
                _ => _.Value.Equals(valueToReverseSubstitute)).Key;
        }
    }
}
