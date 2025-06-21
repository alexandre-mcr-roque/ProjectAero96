using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class ModelAirplane : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ModelName { get; set; } = null!;
        [Column(TypeName = "nvarchar(20)")]
        public string? ModelNameShort { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal PricePerTime { get; set; }
        public ushort MaxSeats { get; set; }

        public override string ToString()
        {
            return ModelNameShort != null
                ? $"{ModelNameShort} - {MaxSeats} seats - {PricePerTime:C} per hour"
                : $"{ModelName} - {MaxSeats} seats - {PricePerTime:C} per hour";
        }
    }
}
