
namespace Project1.Models
{
    public class Trend
    {
        //public int? Id { get; set; }
        public Variable? Variable { get; set; }
        public TrendPoint[]? Data { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
