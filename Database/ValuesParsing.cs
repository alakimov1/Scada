using Project1.Models;
using System.ComponentModel;

namespace Project1.Database
{
    public static class ValuesParsing
    {
        public static object Parse(string value, VariableType? type) =>
            type switch
            {
                VariableType.Bool => bool.Parse(value),
                VariableType.Byte => byte.Parse(value),
                VariableType.Word => int.Parse(value),
                VariableType.Dword => long.Parse(value),
                VariableType.Real => double.Parse(value),
                _ => value
            };

        public static bool TryParse(string value, VariableType? type, out object result)
        {
            switch (type)
            {
                case VariableType.Bool:
                    var boolSuccess = bool.TryParse(value, out bool boolResult);
                    result = boolResult;
                    return boolSuccess;
                case VariableType.Byte:
                    var byteSuccess = byte.TryParse(value, out byte byteResult);
                    result = byteResult;
                    return byteSuccess;
                case VariableType.Word:
                    var intSuccess = int.TryParse(value, out int intResult);
                    result = intResult;
                    return intSuccess;
                case VariableType.Dword:
                    var longSuccess = long.TryParse(value, out long longResult);
                    result = longResult;
                    return longSuccess;
                case VariableType.Real:
                    var doubleSuccess = double.TryParse(value, out double doubleResult);
                    result = doubleResult;
                    return doubleSuccess;
                default:
                    result = value;
                    return true;
            };
        }

        public static bool TryParse(object value, VariableType? type, out object result)
        {
            switch (type)
            {
                case VariableType.Bool:
                    var boolSuccess = value is bool;
                    result = boolSuccess ? (bool)value : null;
                    return boolSuccess;
                case VariableType.Byte:
                    var byteSuccess = value is byte;
                    result = byteSuccess ? (byte)value : null;
                    return byteSuccess;
                case VariableType.Word:
                    var intSuccess = value is int;
                    result = intSuccess ? (int)value : null;
                    return intSuccess;
                case VariableType.Dword:
                    var longSuccess = value is long;
                    result = longSuccess ? (long)value : null;
                    return longSuccess;
                case VariableType.Real:
                    var doubleSuccess = value is double;
                    result = doubleSuccess ? (double)value : null;
                    return doubleSuccess;
                default:
                    result = value;
                    return true;
            };
        }

        public static object GetValueFromString(string value, VariableType type) =>
            type switch
            {
                VariableType.Bool => bool.Parse(value),
                VariableType.Byte => byte.Parse(value),
                VariableType.Word => int.Parse(value),
                VariableType.Dword => long.Parse(value),
                VariableType.Real => double.Parse(value),
                _ => value
            };
    }
}
