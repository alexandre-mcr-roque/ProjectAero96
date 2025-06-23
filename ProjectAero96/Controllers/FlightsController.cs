using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;
using ProjectAero96.Models;

namespace ProjectAero96.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightsRepository flightsRepository;

        public FlightsController(IFlightsRepository flightsRepository)
        {
            this.flightsRepository = flightsRepository;
        }

        [Route("flights")]
        public async Task<IActionResult> Index()
        {
            var cities = await flightsRepository.GetCitiesAsync();
            var model = new FlightViewModel
            {
                Cities = cities.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ToString()
                }).ToList()
            };
            return View(model);
        }

        [Route("flights/{id:int}")]
        public IActionResult Details(int id)
        {
            throw new NotImplementedException("Details method not implemented yet.");
        }

        [Route("flights/create")]
        public async Task<IActionResult> Create()
        {
            var cities = await flightsRepository.GetCitiesAsync();
            var model = new FlightViewModel
            {
                Cities = cities.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ToString()
                }).ToList()
            };
            return View(model);
        }

        [Route("flights/edit")]
        public IActionResult Edit()
        {
            // TODO get list of flights using ajax
            return View();
        }

        [Route("flights/edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await flightsRepository.GetFlightAsync(id).ToFlightViewModelAsync();
            if (model == null)
            {
                return NotFound();
            }
            var cities = await flightsRepository.GetCitiesAsync();
            model.Cities = cities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.ToString()
            }).ToList();
            return View(model);
        }

        [Route("flights/getall")]
        public async Task<JsonResult> GetFlights(int? from, int? to)
        {
            if (from == null || to == null || from.Value == to.Value)
            {
                return Json(new { flights = await flightsRepository.GetAllFlightViewModelsAsync() });
            }
            return Json(new { flights = await flightsRepository.GetFlightViewModelsWithStopsAsync(from.Value, to.Value) });
        }
    }
}
