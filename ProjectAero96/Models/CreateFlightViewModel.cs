using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class CreateFlightViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departure Dates")]
        [MinLength(1, ErrorMessage = "Please enter at least one date.")]
        public ICollection<DateTime> DepartureDates { get; set; } = [];

        //======== Flight Duration =========
        [Required]
        [Display(Name = "Hours")]
        [Range(0, 48, ErrorMessage = "Please enter a valid number of hours ({0}-{1}).")]
        public byte Hours { get; set; }

        [Required]
        [Display(Name = "Minutes")]
        [Range(0, 59, ErrorMessage = "Please enter a valid number of minutes ({0}-{1}).")]
        public byte Minutes { get; set; }


        [Required]
        [Display(Name = "Departure City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the departure city.")]
        public int DepartureCityId { get; set; }

        [Required]
        [Display(Name = "Arrival City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the arrival city.")]
        public int ArrivalCityId { get; set; }

        [Required]
        [Display(Name = "Price ($)")]
        [Range(1, 999999, ErrorMessage = "Please enter a valid price.")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Price Percentage for Children")]
        [Range(1, 100, ErrorMessage = "Please enter a valid percentage between 0 (exclusive) and 100 (inclusive).")]
        public byte ChildPricePercentage { get; set; } = 90;

        [Required]
        [Display(Name = "Price Percentage for Babies")]
        [Range(1, 100, ErrorMessage = "Please enter a valid percentage between 0 (exclusive) and 100 (inclusive).")]
        public byte BabyPricePercentage { get; set; } = 80;

        [Required]
        [Display(Name = "Airplane")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an airplane.")]
        public int AirplaneId { get; set; }

        [JsonIgnore]
        public ICollection<SelectListItem> Cities { get; set; } = [];
        [JsonIgnore]
        public ICollection<SelectListItem> Airplanes { get; set; } = [];

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
