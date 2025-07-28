using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Payment Date")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Departure")]
        public string DepartureCity { get; set; } = null!;

        [Display(Name = "Arrival")]
        public string ArrivalCity { get; set; } = null!;

        [Display(Name = "Number of Tickets")]
        public int NumberTickets { get; set; }

        [Display(Name = "Total")]
        public string TotalPrice { get; set; } = null!;
    }
}
