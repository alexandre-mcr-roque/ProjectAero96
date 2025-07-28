using Microsoft.AspNetCore.Mvc;

namespace ProjectAero96.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/404")]
        public IActionResult Error404() // NotFound
        {
            return View();
        }
    }
}
