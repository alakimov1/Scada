namespace Project1.Controllers.TrendsController
{
    public class TrendsQuery
    {
        public int[] VariablesIds { get; set; }
        public string? Start { get; set; } = null;
        public string? End { get; set; } = null;
    }
}
