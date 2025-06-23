using Microsoft.EntityFrameworkCore;

namespace ProjectAero96.Data.Entities
{
    [PrimaryKey("FlightId","StopIndex")]
    public class FlightStop
    {
        public int FlightId { get; set; }
        public byte StopIndex { get; set; }
        public int CityId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public City City { get; set; } = null!;
        public TimeSpan? FromLastStop { get; set; }
        public TimeSpan? ToNextStop { get; set; }
    }
}