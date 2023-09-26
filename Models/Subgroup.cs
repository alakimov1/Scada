namespace Project1.Models
{
    public class Subgroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Group Group { get; set; }
    }
}
