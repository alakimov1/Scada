using Project1.Models;

namespace Project1.Database
{
    public static class ValuesParsing
    {
        public static object Parse(string value, VariableType? type)
        {
            switch (type)
            {
                case VariableType.Bool: return bool.Parse(value);
                case VariableType.Byte: return byte.Parse(value);
                case VariableType.Word: return int.Parse(value);
                case VariableType.Dword: return long.Parse(value);
                case VariableType.Real: return double.Parse(value);
                default: return value;
            }
        }

        private static object _getValueFromString(string value,VariableType type)
        {
            switch (type)
            {
                case VariableType.Bool: return bool.Parse(value);
                case VariableType.Byte: return byte.Parse(value);
                case VariableType.Word: return int.Parse(value);
                case VariableType.Dword: return long.Parse(value);
                case VariableType.Real: return double.Parse(value);
                default: return value;
            }
        }

    }
}
