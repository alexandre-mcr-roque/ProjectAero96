using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Identity.Client.Extensions.Msal;
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
        private readonly IMailHelper mailHelper;
        private readonly IFileHelper fileHelper;
        private readonly IUserHelper userHelper;

        public FlightsController(IFlightsRepository flightsRepository,
                                 IAirplanesRepository airplanesRepository,
                                 IMailHelper mailHelper,
                                 IFileHelper fileHelper,
                                 IUserHelper userHelper)
        {
            this.flightsRepository = flightsRepository;
            this.airplanesRepository = airplanesRepository;
            this.mailHelper = mailHelper;
            this.fileHelper = fileHelper;
            this.userHelper = userHelper;
        }

        [Route("/flights")]
        public async Task<IActionResult> Index()
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

        [Route("/flights/book/{id:int}")]
        public async Task<IActionResult> BookFlight(int id)
        {
            var flight = await flightsRepository.GetFlightAsync(id);
            if (flight == null)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Flight does not exist.";
                return RedirectToAction("Index");
            }

            var model = new FlightBookingViewModel
            {
                FlightId = flight.Id,
            };
            if (User.Identity!.IsAuthenticated)
            {
                var user = await userHelper.FindUserByEmailAsync(User.Identity.Name!);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.BirthDate = user.BirthDate.ToDateTime(TimeOnly.MinValue);
                    model.Email = user.Email!;
                    model.Tickets.ElementAt(0).FirstName = user.FirstName;
                    model.Tickets.ElementAt(0).LastName = user.LastName;
                    model.Tickets.ElementAt(0).Email = user.Email!;
                    model.Tickets.ElementAt(0).Age = user.BirthDate == default ? 0 : DateTime.Now.Year - user.BirthDate.Year - (DateTime.Now.DayOfYear < user.BirthDate.DayOfYear ? 1 : 0);
                }
            }
            return View(model);
        }

        [Route("/flights/book/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BookFlight(int id, [Bind("FlightId","Tickets","FirstName","LastName","BirthDate","Email","Address1","Address2","City","Country")]FlightBookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            var flight = await flightsRepository.GetFlightAsync(model.FlightId);
            if (flight == null || id != model.FlightId)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Flight does not exist.";
                return RedirectToAction("Index");
            }

            User? user;
            if (User.Identity!.IsAuthenticated)
            {
                user = await userHelper.FindUserByEmailAsync(User.Identity.Name!);
                if (user == null)
                {
                    return NotFound();
                }
            }
            else
            {
                if (await userHelper.FindUserByEmailAsync(model.Email) != null)
                {
                    ViewBag.Summary = FormSummary.Danger("An account with this email already exists. Please sign in to book the flight.");
                    return View(model);
                }
                if (model.BirthDate.Date > DateTime.UtcNow.AddYears(-18))
                {
                    ViewBag.Summary = FormSummary.Danger("You must be at least 18 years old to book a flight.");
                    return View(model);
                }
                user = new User
                {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = DateOnly.FromDateTime(model.BirthDate),
                    Email = model.Email,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    Country = model.Country
                };
                var result = await userHelper.AddUserAsync(user);
                if (!result.Succeeded)
                {
                    ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                    return View(model);
                }

                var token = await userHelper.GenerateResetPasswordTokenAsync(user);
                var tokenLink = Url.Action("SetPassword", "Account", new
                {
                    uid = user.Id,
                    token = await userHelper.GenerateResetPasswordTokenAsync(user)
                }, protocol: HttpContext.Request.Scheme);
                // <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Change Password</a>
                string body = $"""
                    <span style="font-size:2em">Welcome to Aero96</span>
                    <p>
                        Dear {user.FirstName} {user.LastName},<br/>
                        Thank you for booking your flight with us!<br>
                        We've created an account for you to manage your bookings.<br>
                        Please set your password by clicking the link below.
                        <br><br>
                        <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Complete Registration</a>
                    </p>
                    """;
                await mailHelper.SendEmailAsync(user.Email, "Aero96 - Welcome", body);
            }
            var invoice = new Invoice
            {
                FlightId = model.FlightId,
                FlightTickets = model.Tickets.Select(t => new FlightTicket
                {
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    Age = t.Age,
                    Email = t.Email,
                    SeatNumber = t.SeatNumber ?? string.Empty, // TODO method to generate seat number
                    Price = t.Age < 2 ? flight.Price * flight.BabyPricePercentage / 100 :
                            t.Age < 12 ? flight.Price * flight.ChildPricePercentage / 100 : flight.Price
                }).ToList(),
                DepartureDate = flight.DepartureDate,
                DepartureCity = flight.DepartureCity!.ToString(),
                ArrivalCity = flight.ArrivalCity!.ToString(),
                FlightDuration = (flight.Hours, flight.Minutes) switch
                {
                    (_, 0) => $"{flight.Hours} hour{(flight.Hours == 1 ? "" : "s")}",
                    (0, _) => $"{flight.Minutes} minute{(flight.Minutes == 1 ? "" : "s")}",
                    _ => $"{flight.Hours} hour{(flight.Hours == 1 ? "" : "s")} and {flight.Minutes} minute{(flight.Minutes == 1 ? "" : "s")}"
                },
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Country = user.Country
            };
            invoice.TotalPrice = invoice.FlightTickets.Sum(t => t.Price);

            var registerResult = await flightsRepository.RegisterFlightTicketsAsync(invoice);
            if (!registerResult)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }

            // TODO send email with invoice details
            List<IMailFileModel> tickets = new List<IMailFileModel>();
            foreach (var ticket in invoice.FlightTickets)
            {
                var ticketPdf = await fileHelper.GenerateTicketPdfAsync(ticket);
                tickets.Add(ticketPdf);
                if (ticket.Email == invoice.Email)
                {
                    // Skip sending ticket email to the same address as invoice
                    continue;
                }
                string body = $"""
                    <span style="font-size:2em">Flight Booked</span>
                    <p>
                        Thank you for booking your flight with us!<br>
                        Your flight is scheduled for {flight.DepartureDate:dd/MM/yyyy HH:mm} from {flight.DepartureCity!.Name} to {flight.ArrivalCity!.Name}.<br>
                        Your seat number is: {ticket.SeatNumber}.<br><br>
                        We wish you a pleasant journey!
                    </p>
                    """;
                await mailHelper.SendEmailAsync(ticket.Email, "Aero96 - Flight Booked", body, ticketPdf);
            }

            var invoicePdf = await fileHelper.GenerateInvoicePdfAsync(invoice);
            await mailHelper.SendEmailAsync(invoice.Email, "Aero96 - Flight Invoice", "Thank you for your booking. Please find your invoice attached.", invoicePdf, await fileHelper.CombineTicketPdfsAsync(tickets, invoice.CreatedAt.DateTime));

            TempData["SummaryStyle"] = 2;
            TempData["Summary"] = "Flight booked successfully. You will receive an email with the details of your booking.";
            return RedirectToAction("Index");
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
                ViewBag.Summary = FormSummary.Warning("One or more flights failed to be scheduled.");
                return View(model);
            }
            if (result == 1) ViewBag.Summary = FormSummary.Success("Flight scheduled successfully.");
            else ViewBag.Summary = FormSummary.Success("Flights scheduled successfully.");
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
            var flights = await flightsRepository.GetFutureFlightsAsync(from, to);
            return Json(new { flights = flights.ToFlightViewModels() });
        }
    }
}
