using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class FlightTicket
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string SeatNumber { get; set; } = null!;
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }
    }
}
