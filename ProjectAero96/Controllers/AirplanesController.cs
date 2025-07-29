using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;

namespace ProjectAero96.Controllers
{
    [EnumAuthorize(Roles.Employee)]
    /// <summary> Helper controller for getting airplane data. </summary>
    public class AirplanesController : Controller
    {
        private readonly IAirplanesRepository airplanesRepository;

        public AirplanesController(IAirplanesRepository airplanesRepository)
        {
            this.airplanesRepository = airplanesRepository;
        }

        [Route("/airplanes/{id}/flights")]
        public async Task<JsonResult> GetFlightsOfAirplane(int id)
        {
            var flights = await airplanesRepository.GetAllFlightsFromAirplaneAsync(id);
            return Json(new { flights = flights.ToFlightViewModels() });
        }

        [Route("/airplanes/{id}/price-per-hour")]
        public async Task<JsonResult> GetPricePerHourOfAirplane(int id)
        {
            var airplane = await airplanesRepository.GetAirplaneWithModelAsync(id);
            if (airplane == null)
            {
                return Json(null);
            }
            return Json(airplane.AirplaneModel!.PricePerHour);
        }
    }
}
