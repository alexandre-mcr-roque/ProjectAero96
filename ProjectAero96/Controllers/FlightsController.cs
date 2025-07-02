using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Entities;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;
using ProjectAero96.Models;

namespace ProjectAero96.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightsRepository flightsRepository;
        private readonly IAirplanesRepository airplanesRepository;

        public FlightsController(IFlightsRepository flightsRepository,
                                 IAirplanesRepository airplanesRepository)
        {
            this.flightsRepository = flightsRepository;
            this.airplanesRepository = airplanesRepository;
        }

        [Route("/flights")]
        public async Task<IActionResult> Index()
        {
            var model = new FlightViewModel
            {
                Cities = await flightsRepository.GetCitySelectListItemsAsync()
            };
            return View(model);
        }

        [Route("/flights/{id:int}")]
        public IActionResult Details(int id)
        {
            throw new NotImplementedException("Details method not implemented yet.");
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/schedule")]
        public async Task<IActionResult> Create()
        {
            var model = new FlightViewModel
            {
                Cities = await flightsRepository.GetCitySelectListItemsAsync(),
                Airplanes = await flightsRepository.GetAirplaneSelectListItemsAsync()
            };
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/schedule")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DayOfWeek","DepartureTime","FlightDuration","DepartureCityId","ArrivalCityId","Price","ChildPricePercentage","BabyPricePercentage","AirplaneId")]FlightViewModel model)
        {
            model.Cities = await flightsRepository.GetCitySelectListItemsAsync();
            model.Airplanes = await flightsRepository.GetAirplaneSelectListItemsAsync();
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            var airplane = await airplanesRepository.GetAirplaneAsync(model.AirplaneId);
            if (airplane == null)
            {
                ViewBag.Summary = FormSummary.Danger("Airplane does not exist.");
                model.AirplaneId = 0;
                return View(model);
            }

            var departureCity = await flightsRepository.GetCityAsync(model.DepartureCityId);
            if (departureCity == null)
            {
                ViewBag.Summary = FormSummary.Danger("Departure city does not exist.");
                model.DepartureCityId = 0;
                return View(model);
            }

            var arrivalCity = await flightsRepository.GetCityAsync(model.ArrivalCityId);
            if (arrivalCity == null)
            {
                ViewBag.Summary = FormSummary.Danger("Arrival city does not exist.");
                model.ArrivalCityId = 0;
                return View(model);
            }

            var flights = await airplanesRepository.GetAllFlightsFromAirplaneAsync(model.AirplaneId);
            // TODO validate if the timespan of the flight is valid (no other flights scheduled for the same plane at the same time)

            var flight = new Flight
            {
                DayOfWeek = model.DayOfWeek,
                DepartureTime = TimeOnly.Parse(model.DepartureTime),
                FlightDuration = TimeSpan.Parse(model.FlightDuration),
                DepartureCityId = model.DepartureCityId,
                ArrivalCityId = model.ArrivalCityId,
                Price = model.Price,
                ChildPricePercentage = model.ChildPricePercentage,
                BabyPricePercentage = model.BabyPricePercentage,
                AirplaneId = model.AirplaneId
            };
            var result = await flightsRepository.AddFlightAsync(flight);
            if (!result)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            ViewBag.Summary = FormSummary.Success("Flight created successfully.");
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/edit")]
        public IActionResult Edit()
        {
            // TODO get list of flights using ajax
            return View();
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await flightsRepository.GetFlightAsync(id).ToFlightViewModelAsync();
            if (model == null)
            {
                return NotFound();
            }
            model.Cities = await flightsRepository.GetCitySelectListItemsAsync();
            return View(model);
        }

        [Route("/flights/getall")]
        public async Task<JsonResult> GetFlights(int? from, int? to)
        {
            var flights = await flightsRepository.GetAllFlightsAsync(from, to);
            return Json(new { flights = flights.ToFlightViewModels() });
        }
    }
}
