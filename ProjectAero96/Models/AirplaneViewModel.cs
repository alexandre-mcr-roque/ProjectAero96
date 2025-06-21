using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectAero96.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class AirplaneViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Airline")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string Airline { get; set; } = null!;

        [Display(Name = "Description (optional)")]
        [MaxLength(255, ErrorMessage = "This field must have {0} characters or less.")]
        public string? Description { get; set; }
        [Display(Name = "Description")]
        public string DescriptionStr => Description ?? AirplaneModel?.ToString() ?? "(No description given)";

        [Required]
        [Display(Name = "First Class Seats")]
        public ushort FCSeats { get; set; }

        [Required]
        [Display(Name = "Economy Seats")]
        public ushort ESeats { get; set; }

        [Display(Name = "Airline Image (optional)")]
        public IFormFile? AirlineImage { get; set; }
        public string? AirlineImageId { get; set; }
        public string AirlineImagePath => $"https://aero96sa.blob.core.windows.net/airline-images/{AirlineImageId}";

        [Required]
        [Display(Name = "Airplane Model")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an airplane model.")]
        public int AirplaneModelId { get; set; }
        public ModelAirplane? AirplaneModel { get; set; }
        public ICollection<SelectListItem>? AirplaneModels { get; set; }

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
