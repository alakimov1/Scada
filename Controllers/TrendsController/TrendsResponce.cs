using Project1.Models;

namespace Project1.Controllers.TrendsController
{
    public class TrendsResponce
    {
        public double[]? Warnings { get; set; }
        public double[]? Alarms { get; set; }
        public Trend? Trend { get; set; }
    }
}
