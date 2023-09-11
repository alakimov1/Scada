namespace Project1.Controllers.EventsController
{
    public class EventsQuery
    {
        public int? VariableId { get; set; } = null;
        public string? Start { get; set; } = null;
        public string? End { get; set; } = null;
        public int[]? Type { get; set; } = null;
        public int? Count { get; set; } = 0;
    }
}
