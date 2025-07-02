using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Day of the Week")]
        [Range(0, 6, ErrorMessage = "Please select the day of the week.")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        [RegularExpression(@"^([0-1]?\d|2[0-3])(:[0-5]\d)$", ErrorMessage = "Please enter a valid departure time. (hh:mm)")]
        public string DepartureTime { get; set; } = "14:00";

        [Required]
        [Display(Name = "Flight Duration")]
        [RegularExpression(@"^([1-9]\d*\.00:00|([1-9]\d*\.)?((0{1,2}:(0[1-9]|[1-5]\d))|((0?[1-9]|1\d|2[0-3]):[0-5]\d)))$")]
        public string FlightDuration { get; set; } = "1:00";

        [Required]
        [Display(Name = "Departure City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the departure city.")]
        public int DepartureCityId { get; set; }
        [JsonIgnore]
        public CityViewModel? DepartureCity { get; set; } = null!;

        [Required]
        [Display(Name = "Arrival City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the arrival city.")]
        public int ArrivalCityId { get; set; }
        [JsonIgnore]
        public CityViewModel? ArrivalCity { get; set; } = null!;

        [Required]
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
        public AirplaneViewModel? Airplane { get; set; }

        [JsonIgnore]
        public ICollection<SelectListItem> Cities { get; set; } = [];
        [JsonIgnore]
        public ICollection<SelectListItem> Airplanes { get; set; } = [];

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
