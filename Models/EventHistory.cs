namespace Project1.Models
{
    public class EventHistory
    {
        public int? Id { get; set; }
        public Event? Event { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
