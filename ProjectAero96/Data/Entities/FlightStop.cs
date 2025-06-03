using Microsoft.EntityFrameworkCore;

namespace ProjectAero96.Data.Entities
{
    [PrimaryKey("FlightId","StopIndex")]
    public class FlightStop
    {
        public int FlightId { get; set; }
        public byte StopIndex { get; set; }
        public City City { get; set; } = null!;
        public TimeOnly? FromLastStop { get; set; }
        public TimeOnly? ToNextStop { get; set; }
    }
}