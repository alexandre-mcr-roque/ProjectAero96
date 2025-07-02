using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectAero96.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectAero96.Models
{
    public class AirplaneViewModel : ISeatConfigurationModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Airline")]
        [MaxLength(100, ErrorMessage = "This field must have {0} characters or less.")]
        public string Airline { get; set; } = null!;

        [JsonIgnore]
        [Display(Name = "Description")]
        [MaxLength(255, ErrorMessage = "This field must have {0} characters or less.")]
        public string? Description { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string DescriptionStr => Description ?? AirplaneModel?.ToString() ?? "(No description given)";

        [JsonIgnore]
        [Display(Name = "Airline Image (optional)")]
        public IFormFile? AirlineImage { get; set; }
        public string? AirlineImageId { get; set; }
        public string AirlineImagePath => $"https://aero96sa.blob.core.windows.net/airline-images/{AirlineImageId}";

        [Required]
        [Display(Name = "Airplane Model")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an airplane model.")]
        public int AirplaneModelId { get; set; }
        [JsonIgnore]
        public ModelAirplaneViewModel? AirplaneModel { get; set; }
        [JsonIgnore]
        public ICollection<SelectListItem>? AirplaneModels { get; set; }

        [JsonIgnore]
        public ushort MaxSeats { get; set; }

        [Required]
        [Display(Name = "Number of Seat Rows")]
        [Range(1, 1000, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public ushort SeatRows { get; set; } = 1;

        [Required]
        [Display(Name = "Number of Seat Columns")]
        [Range(1, 12, ErrorMessage = "The maximum amount of seat columns must be between {1} and {2}.")]
        public byte SeatColumns { get; set; } = 1;

        [Required]
        [Display(Name = "Number of Seats per Window")]
        [Range(1, 4, ErrorMessage = "The maximum amount of seats must be between {1} and {2}.")]
        public byte WindowSeats { get; set; } = 1;

        [Display(Name = "Is Disabled?")]
        public bool Deleted { get; set; }
    }
}
