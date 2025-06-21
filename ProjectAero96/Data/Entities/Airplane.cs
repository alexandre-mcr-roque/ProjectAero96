using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class Airplane : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Airline { get; set; } = null!;

        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }

        public ushort FCSeats { get; set; } // First Class Seats
        public ushort ESeats { get; set; } // Economy Seats

        [Column(TypeName = "nvarchar(450)")]
        public string? AirlineImageId { get; set; }

        public int AirplaneModelId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ModelAirplane AirplaneModel { get; set; } = null!;
    }
}
