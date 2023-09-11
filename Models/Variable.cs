namespace Project1.Models
{
    public enum VariableType
    {
        None,
        Bool,
        Byte,
        Word,
        Dword,
        Real,
        String
    }

    public class Variable
    {
        public int? Id { get; set; }
        public int Address { get; set; }
        public VariableType Type { get; set; }
        public string Name { get; set; } = "";
        public object? Value { get; set; }    
        public int Active { get; set; }
        public int? TrendingPeriod { get; set; }
    }
}
