namespace ProjectAero96.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; } = null!;
        public string Country { get; set; } = null!;
        public override string ToString()
        {
            return $"{Name}, {Country}";
        }
    }
}
