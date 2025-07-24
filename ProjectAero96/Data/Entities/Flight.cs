using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAero96.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public DateTimeOffset DepartureDate { get; set; }
        
        //======== Flight Duration =========
        public byte Hours { get; set; }
        public byte Minutes { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }

        public int DepartureCityId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public City? DepartureCity { get; set; }

        public int ArrivalCityId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public City? ArrivalCity { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }
        public byte ChildPricePercentage { get; set; }
        public byte BabyPricePercentage { get; set; }

        public int AirplaneId { get; set; }
        public Airplane? Airplane { get; set; }
    }
}
