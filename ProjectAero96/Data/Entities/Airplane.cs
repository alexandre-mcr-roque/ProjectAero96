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
        public string Description { get; set; } = null!;

        [Column(TypeName = "nvarchar(450)")]
        public string? AirlineImageId { get; set; }

        public int AirplaneModelId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ModelAirplane? AirplaneModel { get; set; }
        public ushort MaxSeats { get; set; }
        public ushort SeatRows { get; set; }
        public byte SeatColumns { get; set; }
        public byte WindowSeats { get; set; }
    }
}
