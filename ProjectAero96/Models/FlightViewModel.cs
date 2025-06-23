using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public DayOfWeek DaysOfWeek { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan? ReturnTime { get; set; }
        public bool HasReturn => ReturnTime.HasValue;
        public decimal Price { get; set; }
        public decimal PricePerTime { get; set; }
        public float? ChildPriceModifier { get; set; }
        public float? BabyPriceModifier { get; set; }
        public int AirplaneId { get; set; }
        public AirplaneViewModel? Airplane { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ICollection<FlightStop> FlightStops { get; set; } = [];
        public ICollection<SelectListItem> Cities { get; set; } = [];
    }
}
