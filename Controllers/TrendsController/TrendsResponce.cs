using Project1.Models;

namespace Project1.Controllers.TrendsController
{
    public class TrendsResponceEventLine
    {
        public EventType? EventType { get; set; }
        public double? Value { get; set; }
    }

    public class TrendsResponce
    {
        public TrendsResponceEventLine[]? EventLines { get; set; }
        public Trend? Trend { get; set; }
    }
}
