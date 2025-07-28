using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;

namespace ProjectAero96.Controllers.API
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FlightsController : ControllerBase
    {
        private readonly IUserHelper userHelper;
        private readonly IFlightsRepository flightsRepository;

        public FlightsController(IUserHelper userHelper, IFlightsRepository flightsRepository)
        {
            this.userHelper = userHelper;
            this.flightsRepository = flightsRepository;
        }

        [Route("/api/flights/booked")]
        public async Task<IActionResult> GetBookedFlights()
        {
            var user = await userHelper.FindUserByEmailAsync(User.Claims.First().Value);
            if (user == null)
            {
                return NotFound();
            }
            
            var result = await flightsRepository.GetBookedFlightsOfUserAsync(user);
            if (result == null || result.Count == 0)
            {
                return Ok(new { message = "There are no flights booked."});
            }
            return Ok(new { flights = result.Select(f => new
            {
                DepartureDate = f.DepartureDate,
                DepartureDateUTC = f.DepartureDate.UtcDateTime,
                DepartureCity = new { City = f.DepartureCity!.Name, Country = f.DepartureCity.Country },
                ArrivalCity = new { City = f.ArrivalCity!.Name, Country = f.ArrivalCity.Country },
                FlightDuration = $"{f.Hours:00}:{f.Minutes:00}",
                PricePerTicket = f.Price
            })});
        }
    }
}
