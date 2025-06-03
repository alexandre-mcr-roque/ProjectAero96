using Microsoft.EntityFrameworkCore;

namespace ProjectAero96.Data.Entities
{
    public class Airplane : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public ushort FCSeats { get; set; } // First Class Seats
        public ushort ESeats { get; set; } // Economy Seats
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ModelAirplane AirplaneModel { get; set; } = null!;
    }
}
