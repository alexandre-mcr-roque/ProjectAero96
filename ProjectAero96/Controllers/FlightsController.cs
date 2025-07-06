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

        [Route("/flights/book/{id:int}")]
        public async Task<IActionResult> Book(int id)
        {
            var flight = await flightsRepository.GetFlightAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            throw new NotImplementedException("Book method not implemented yet.");
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

            if (model.DepartureCityId == model.ArrivalCityId)
            {
                ViewBag.Summary = FormSummary.Danger("Departure city and arrival city cannot be the same.");
                return View(model);
            }

            if (model.Hours == 0 && model.Minutes == 0)
            {
                ViewBag.Summary = FormSummary.Danger("Flight duration cannot be zero.");
                return View(model);
            }

            if (model.Hours == 48 && model.Minutes > 0)
            {
                ViewBag.Summary = FormSummary.Danger("Flight duration cannot be more than 48 hours.");
                return View(model);
            }

            var flights = await airplanesRepository.GetAllFlightsFromAirplaneAsync(model.AirplaneId);
            var dayOfWeek = model.DayOfWeek;
            var departureTime = TimeOnly.Parse(model.DepartureTime);
            var arrivalTime = departureTime.AddHours(model.Hours).AddMinutes(model.Minutes);
            var arrivalDayOfWeek = arrivalTime < departureTime ? (DayOfWeek)(((int)dayOfWeek + 1) % 7) : dayOfWeek;

            // Helper to get absolute minutes since week start for comparison
            int GetMinutesOfWeek(DayOfWeek day, TimeOnly time) => ((int)day * 1440) + (time.Hour * 60) + time.Minute;

            // New flight's interval in minutes since week start
            int start = GetMinutesOfWeek(dayOfWeek, departureTime);
            int end = GetMinutesOfWeek(arrivalDayOfWeek, arrivalTime);

            // Handle flights that wrap around the week (e.g., Sunday night to Monday morning)
            if (end <= start) end += 10080; // 7 * 1440

            foreach (var f in flights)
            {
                var fDayOfWeek = f.DayOfWeek;
                var fDepartureTime = f.DepartureTime;
                var fArrivalTime = fDepartureTime.AddHours(f.Hours).AddMinutes(f.Minutes);
                var fArrivalDayOfWeek = fArrivalTime < fDepartureTime ? (DayOfWeek)(((int)fDayOfWeek + 1) % 7) : fDayOfWeek;

                int fStart = GetMinutesOfWeek(fDayOfWeek, fDepartureTime);
                int fEnd = GetMinutesOfWeek(fArrivalDayOfWeek, fArrivalTime);

                if (fEnd <= fStart)
                    fEnd += 10080; // 7 * 1440

                // Check for overlap
                if (start < fEnd && end > fStart)
                {
                    ViewBag.Summary = FormSummary.Danger("There is already a flight scheduled for this airplane that overlaps with the selected time.");
                    return View(model);
                }
            }

            var flight = new Flight
            {
                DayOfWeek = model.DayOfWeek,
                DepartureTime = departureTime,
                Hours = model.Hours,
                Minutes = model.Minutes,
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
        public async Task<IActionResult> Edit()
        {
            string? summary = (string?)TempData["Summary"];
            if (summary != null)
            {
                int style = (int)TempData["SummaryStyle"]!;
                ViewBag.Summary = FormSummary.FromCode(style, summary);
            }
            var model = new FlightViewModel
            {
                Cities = await flightsRepository.GetCitySelectListItemsAsync()
            };
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/edit/{id:int}")]
        public async Task<IActionResult> EditFlight(int id)
        {
            var model = await flightsRepository.GetFlightAsync(id).ToFlightViewModelAsync();
            if (model == null)
            {
                return NotFound();
            }
            model.Cities = await flightsRepository.GetCitySelectListItemsAsync();
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFlight(int id, [Bind("DayOfWeek","DepartureTime","FlightDuration","DepartureCityId","ArrivalCityId","Price","ChildPricePercentage","BabyPricePercentage","AirplaneId","Deleted")]FlightViewModel model)
        {
            model.Cities = await flightsRepository.GetCitySelectListItemsAsync();
            model.Airplanes = await flightsRepository.GetAirplaneSelectListItemsAsync();
         
            if (model.Id != id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            var flight = await flightsRepository.GetFlightAsync(model.Id);
            if (flight == null)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Flight does not exist.";
                return RedirectToAction("Edit");
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

            if (model.DepartureCityId == model.ArrivalCityId)
            {
                ViewBag.Summary = FormSummary.Danger("Departure city and arrival city cannot be the same.");
                return View(model);
            }

            if (model.Hours == 0 && model.Minutes == 0)
            {
                ViewBag.Summary = FormSummary.Danger("Flight duration cannot be zero.");
                return View(model);
            }

            if (model.Hours == 48 && model.Minutes > 0)
            {
                ViewBag.Summary = FormSummary.Danger("Flight duration cannot be more than 48 hours.");
                return View(model);
            }

            var flights = await airplanesRepository.GetAllFlightsFromAirplaneAsync(model.AirplaneId);
            var dayOfWeek = model.DayOfWeek;
            var departureTime = TimeOnly.Parse(model.DepartureTime);
            var arrivalTime = departureTime.AddHours(model.Hours).AddMinutes(model.Minutes);
            var arrivalDayOfWeek = arrivalTime < departureTime ? (DayOfWeek)(((int)dayOfWeek + 1) % 7) : dayOfWeek;

            // Helper to get absolute minutes since week start for comparison
            int GetMinutesOfWeek(DayOfWeek day, TimeOnly time) => ((int)day * 1440) + (time.Hour * 60) + time.Minute;

            // New flight's interval in minutes since week start
            int start = GetMinutesOfWeek(dayOfWeek, departureTime);
            int end = GetMinutesOfWeek(arrivalDayOfWeek, arrivalTime);

            // Handle flights that wrap around the week (e.g., Sunday night to Monday morning)
            if (end <= start) end += 10080; // 7 * 1440

            foreach (var f in flights)
            {
                var fDayOfWeek = f.DayOfWeek;
                var fDepartureTime = f.DepartureTime;
                var fArrivalTime = fDepartureTime.AddHours(f.Hours).AddMinutes(f.Minutes);
                var fArrivalDayOfWeek = fArrivalTime < fDepartureTime ? (DayOfWeek)(((int)fDayOfWeek + 1) % 7) : fDayOfWeek;

                int fStart = GetMinutesOfWeek(fDayOfWeek, fDepartureTime);
                int fEnd = GetMinutesOfWeek(fArrivalDayOfWeek, fArrivalTime);

                if (fEnd <= fStart)
                    fEnd += 10080; // 7 * 1440

                // Check for overlap
                if (start < fEnd && end > fStart)
                {
                    ViewBag.Summary = FormSummary.Danger("There is already a flight scheduled for this airplane that overlaps with the selected time.");
                    return View(model);
                }
            }

            //TODO check if flight is in use (booked tickets for future flights)
            flight.DayOfWeek = model.DayOfWeek;
            flight.DepartureTime = departureTime;
            flight.Hours = model.Hours;
            flight.Minutes = model.Minutes;
            flight.DepartureCityId = model.DepartureCityId;
            flight.ArrivalCityId = model.ArrivalCityId;
            flight.Price = model.Price;
            flight.ChildPricePercentage = model.ChildPricePercentage;
            flight.BabyPricePercentage = model.BabyPricePercentage;
            flight.AirplaneId = model.AirplaneId;
            flight.Deleted = model.Deleted;
            var result = await flightsRepository.UpdateFlightAsync(flight);
            if (!result)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            ViewBag.Summary = FormSummary.Success("Flight created successfully.");
            return View(model);
        }

        [Route("/flights/getall")]
        public async Task<JsonResult> GetFlights(int? from, int? to)
        {
            var flights = await flightsRepository.GetAllFlightsAsync(from, to);
            return Json(new { flights = flights.ToFlightViewModels() });
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/getall/include-deleted")]
        public async Task<JsonResult> GetFlightsWithDeleted(int? from, int? to)
        {
            var flights = await flightsRepository.GetAllFlightsWithDeletedAsync(from, to);
            return Json(new { flights = flights.ToFlightViewModels() });
        }
    }
}
