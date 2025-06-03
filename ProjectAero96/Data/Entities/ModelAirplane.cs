using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class ModelAirplane : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string ModelName { get; set; } = null!;
        [Column(TypeName = "decimal(8,2)")]
        public decimal PricePerTime { get; set; }
        public ushort MaxSeats { get; set; }
    }
}
