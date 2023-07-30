namespace Project1.Models
{
    public enum EventType
    {
        Alarm,
        Warning,
        Event
    }
    public enum EventVariableComparison
    {
        None,
        Equal, 
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Empty,
        Contain,
        NotContain
    }

    public class Event
    {
        public int? Id { get; set; }
        public EventType? Type { get; set; }
        public Variable? Variable { get; set; }
        public object? Limit { get; set; }
        public EventVariableComparison? Comparison { get; set; }
        public string? Message { get; set; }
        
        public bool Check()
        {
            switch (Variable?.Type)
            {
                case VariableType.None: return false;
                case VariableType.Bool: return _boolComparator();
                case VariableType.Byte: return _longComparator();
                case VariableType.Word: return _longComparator();
                case VariableType.Dword: return _longComparator();
                case VariableType.Real: return _doubleComparator();
                case VariableType.String: return _stringComparator();
                default: return false;
            }
        }

        private bool _boolComparator()
        {
            var variable = (bool?)Variable?.Value;
            var limit = (bool?)Limit;

            if (variable == null
                && limit == null)
            { 
                return true;
            }

            if (variable==null
                || limit ==null)
            {
                return false;
            }

            switch (Comparison)
            {
                case EventVariableComparison.None: return false;
                case EventVariableComparison.Equal: return variable == limit;
                case EventVariableComparison.NotEqual: return variable != limit;
                default: return false;
            }
        }
        private bool _longComparator()
        {
            if (Variable?.Value == null
                && Limit == null)
            {
                return true;
            }

            if (Variable?.Value == null
                || Limit == null)
            {
                return false;
            }

            var variable = Convert.ToInt64(Variable?.Value);
            var limit = Convert.ToInt64(Limit);

            switch (Comparison)
            {
                case EventVariableComparison.None: return false;
                case EventVariableComparison.Equal: return variable == limit;
                case EventVariableComparison.NotEqual: return variable != limit;
                case EventVariableComparison.GreaterThan: return variable > limit;
                case EventVariableComparison.LessThan: return variable < limit;
                case EventVariableComparison.LessThanOrEqual: return variable <= limit;
                case EventVariableComparison.GreaterThanOrEqual: return variable >= limit;
                default: return false;
            }
        }
        private bool _doubleComparator()
        {
            var variable = (double?)Variable?.Value;
            var limit = (double?)Limit;

            if (variable == null
                && limit == null)
            {
                return true;
            }

            if (variable == null
                || limit == null)
            {
                return false;
            }

            switch (Comparison)
            {
                case EventVariableComparison.Equal: return variable == limit;
                case EventVariableComparison.NotEqual: return variable != limit;
                case EventVariableComparison.GreaterThan: return variable > limit;
                case EventVariableComparison.LessThan: return variable < limit;
                case EventVariableComparison.LessThanOrEqual: return variable <= limit;
                case EventVariableComparison.GreaterThanOrEqual: return variable >= limit;
                default: return false;
            }
        }

        private bool _stringComparator()
        {
            var variable = (string?)Variable?.Value;
            var limit = (string?)Limit;

            if (variable == null
                && limit == null)
            {
                return true;
            }

            if (variable == null
                || limit == null)
            {
                return false;
            }

            switch (Comparison)
            {
                case EventVariableComparison.Equal: return variable == limit;
                case EventVariableComparison.NotEqual: return variable != limit;
                case EventVariableComparison.Empty: return string.IsNullOrEmpty(variable);
                case EventVariableComparison.Contain: return variable.Contains(limit);
                case EventVariableComparison.NotContain: return !variable.Contains(limit);
                default: return false;
            }
        }

    }
}
