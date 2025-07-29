using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Models;
using System.Diagnostics;

namespace ProjectAero96.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/api")]
        public IActionResult API()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/test")]
        public IActionResult Test()
        {
            return View();
        }

        [Route("test/flights/occupied/1")]
        public IActionResult TestSeats()
        {
            var values = new string[]
            {
                "A2", "A3", "A6", "A13", "A21", "A32", "A42",
                "B1", "B4", "B5", "B7", "B8", "B9", "B10", "B11", "B12", "B14", "B15", "B18", "B19", "B20", "B22", "B23", "B24", "B26", "B27",
                "C2", "C3", "C4", "C6", "C8", "C9", "C10", "C11", "C12", "C13", "C15", "C16", "C18", "C19",
                "D2", "D3", "D4", "D5", "D6", "D8", "D9", "D10", "D11", "D12", "D13", "D16", "D18", "D19", "D21", "D22", "D23", "D24", "D27", "D28", "D29", "D30", "D32"
            };
            return Json(values);
        }
    }
}
