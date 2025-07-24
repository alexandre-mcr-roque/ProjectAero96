using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly IUserHelper userHelper;

        public FlightsController(IFlightsRepository flightsRepository,
                                 IAirplanesRepository airplanesRepository,
                                 IUserHelper userHelper)
        {
            this.flightsRepository = flightsRepository;
            this.airplanesRepository = airplanesRepository;
            this.userHelper = userHelper;
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
            // TODO https://developer.mozilla.org/en-US/docs/Web/API/Web_Storage_API/Using_the_Web_Storage_API
            var flight = await flightsRepository.GetFlightAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            var model = new FlightBookingViewModel
            {
                FlightId = flight.Id
            };
            if (User.Identity!.IsAuthenticated)
            {
                var user = await userHelper.FindUserByEmailAsync(User.Identity.Name!);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.BirthDate = user.BirthDate;
                    model.Email = user.Email!;
                    model.Tickets.Add(new FlightBookingViewModel.FlightTicket
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Age = user.Age,
                        Email = user.Email!
                    });
                }

            }
            // Add a default empty ticket if no tickets have been added
            // (most likely an anonymous booking attempt)
            if (model.Tickets.Count == 0)
            {
                model.Tickets.Add(new FlightBookingViewModel.FlightTicket
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Age = 0,
                    Email = string.Empty
                });
            }
            return View(model);
        }


        //=======================================================
        // CRUD
        //=======================================================
        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/schedule")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateFlightViewModel
            {
                Cities = await flightsRepository.GetCitySelectListItemsAsync(),
                Airplanes = await flightsRepository.GetAirplaneSelectListItemsAsync()
            };
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/schedule")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartureDates","Hours","Minutes","DepartureCityId","ArrivalCityId","Price","ChildPricePercentage","BabyPricePercentage","AirplaneId")]CreateFlightViewModel model)
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

            // Check if flights overlap eachother
            if (model.DepartureDates.Count > 1)
            {
                // Calculate intervals for each departure date
                var intervals = model.DepartureDates
                    .Select(d => new
                    {
                        Start = d,
                        End = d.AddHours(model.Hours).AddMinutes(model.Minutes)
                    })
                    .OrderBy(i => i.Start)
                    .ToList();

                var overlap = await Task.Run(() =>
                {
                    for (int i = 0; i < intervals.Count - 1; i++)
                    {
                        for (int j = i + 1; j < intervals.Count; j++)
                        {
                            // If the next interval starts before the current one ends, overlap
                            if (intervals[j].Start < intervals[i].End)
                                return true;
                        }
                    }
                    return false;
                });
                if (overlap)
                {
                    ViewBag.Summary = FormSummary.Danger("There is an overlap between the selected departure dates.");
                    model.DepartureDates.Clear();
                    return View(model);
                }
            }

            var flights = new List<Flight>();
            foreach (var departureDate in model.DepartureDates)
            {
                if (departureDate < DateTime.UtcNow.AddDays(30))
                {
                    ViewBag.Summary = FormSummary.Danger("Cannot schedule a flight in under 30 days.");
                    model.DepartureDates.Clear();
                    return View(model);
                }
                var arrivalDate = departureDate.AddHours(model.Hours).AddMinutes(model.Minutes);
                var existingFlights = await airplanesRepository.GetOverlappingFlightsFromAirplaneAsync(model.AirplaneId, departureDate, arrivalDate);
                if (existingFlights.Count > 0)
                {
                    ViewBag.Summary = FormSummary.Danger("There is already a flight scheduled for this airplane that overlaps with one or more selected dates.");
                    return View(model);
                }

                var newFlight = new Flight
                {
                    DepartureDate = new DateTimeOffset(departureDate.Year, departureDate.Month, departureDate.Day, departureDate.Hour, departureDate.Minute, 0, TimeSpan.Zero),
                    Hours = model.Hours,
                    Minutes = model.Minutes,
                    ArrivalDate = arrivalDate,
                    DepartureCityId = model.DepartureCityId,
                    ArrivalCityId = model.ArrivalCityId,
                    Price = model.Price,
                    ChildPricePercentage = model.ChildPricePercentage,
                    BabyPricePercentage = model.BabyPricePercentage,
                    AirplaneId = model.AirplaneId
                };
                flights.Add(newFlight);
            }
            // Clear departure dates
            model.DepartureDates.Clear();
            var result = await flightsRepository.AddFlightsAsync(flights);
            if (result == 0)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            if (result < flights.Count)
            {
                ViewBag.Summary = FormSummary.Warning("One or more flights failed to be created.");
                return View(model);
            }
            if (result == 1) ViewBag.Summary = FormSummary.Success("Flight created successfully.");
            else ViewBag.Summary = FormSummary.Success("Flights created successfully.");
            return View(model);
        }

        [EnumAuthorize(Roles.Employee)]
        [Route("/flights/unschedule/{id}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await flightsRepository.GetFlightAsync(id);
            if (flight == null)
            {
                return NotFound("Flight does not exist");
            }
            if (await flightsRepository.HasFlightTicketsAsync(flight))
            {
                // In a real scenario, we would add a refund process here instead of preventing deletion
                return NotFound("Flight already has reservations and cannot be unscheduled");
            }
            var result = await flightsRepository.DeleteFlightAsync(flight);
            if (!result)
            {
                return NotFound("Unknown error");
            }
            return Ok(flight);
        }

        [Route("/flights/getall")]
        public async Task<JsonResult> GetFlights(int? from, int? to)
        {
            var flights = await flightsRepository.GetAllFlightsAsync(from, to);
            return Json(new { flights = flights.ToFlightViewModels() });
        }
    }
}
