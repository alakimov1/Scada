namespace Project1.Models
{
    public class VariableEntity
    {
        public int? Id { get; set; }
        public Subgroup Subgroup { get; set; }  
        public Variable Variable { get; set; }
        public bool Writable { get; set; }
    }
}
