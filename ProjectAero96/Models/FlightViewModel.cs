﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }


        //======== Flight Duration =========
        [Required]
        [Display(Name = "Hours")]
        public byte Hours { get; set; }

        [Required]
        [Display(Name = "Minutes")]
        public byte Minutes { get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }

        [Display(Name = "Flight Duration")]
        public string FlightDuration => (Hours, Minutes) switch
        {
            (0, _) => $"{Minutes} minute{(Minutes == 1 ? "" : "s")}",
            (_, 0) => $"{Hours} hour{(Hours == 1 ? "" : "s")}",
            _ => $"{Hours} hour{(Hours == 1 ? "" : "s")} and {Minutes} minute{(Minutes == 1 ? "" : "s")}"
        };

        [Required]
        [Display(Name = "Departure City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the departure city.")]
        public int DepartureCityId { get; set; }
        [JsonIgnore]
        [Display(Name = "Departure")]
        public CityViewModel? DepartureCity { get; set; } = null!;

        [Required]
        [Display(Name = "Arrival City")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select the arrival city.")]
        public int ArrivalCityId { get; set; }
        [JsonIgnore]
        [Display(Name = "Arrival")]
        public CityViewModel? ArrivalCity { get; set; } = null!;

        [Required]
        [Display(Name = "Price")]
        public string Price { get; set; } = null!;

        [Required]
        [Display(Name = "Price Percentage for Children")]
        [Range(1, 100, ErrorMessage = "Please enter a valid percentage between 0 (exclusive) and 100 (inclusive).")]
        public byte ChildPricePercentage { get; set; }

        [Required]
        [Display(Name = "Price Percentage for Babies")]
        [Range(1, 100, ErrorMessage = "Please enter a valid percentage between 0 (exclusive) and 100 (inclusive).")]
        public byte BabyPricePercentage { get; set; }

        [Required]
        [Display(Name = "Airplane")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an airplane.")]
        public int AirplaneId { get; set; }
        [JsonIgnore]
        public AirplaneViewModel? Airplane { get; set; }

        [JsonIgnore]
        public ICollection<SelectListItem> Cities { get; set; } = [];

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
