using Microsoft.EntityFrameworkCore;

namespace ProjectAero96.Data.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public ICollection<FlightTicket> FlightTickets { get; set; } = [];
        public DateOnly DepartureDay { get; set; }
        public int FlightId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Flight? Flight { get; set; } = null!;
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
