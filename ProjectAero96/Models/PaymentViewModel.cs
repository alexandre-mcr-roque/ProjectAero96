using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class PaymentViewModel : FlightBookingViewModel
    {
        [Required(ErrorMessage = "Please select a payment method.")]
        public string PaymentMethod { get; set; } = null!;
    }
}
