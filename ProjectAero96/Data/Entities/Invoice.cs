using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public ICollection<FlightTicket> FlightTickets { get; set; } = [];
        public DateTimeOffset DepartureDate { get; set; }
        public int FlightId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Flight? Flight { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        [Column(TypeName = "decimal(8,2)")]
        public decimal TotalPrice { get; set; }
    }
}
