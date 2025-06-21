using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        [Column(TypeName = "tinyint")]
        public DayOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan? ReturnTime { get; set; }
        [Column(TypeName = "decimal(8,2)")]
        public decimal PricePerTime { get; set; }
        public float? ChildPriceModifier { get; set; }
        public float? BabyPriceModifier { get; set; }
        public int AirplaneId { get; set; }
        public Airplane Airplane { get; set; } = null!;
        public ICollection<FlightStop>? FlightStops { get; set; }
    }
}
