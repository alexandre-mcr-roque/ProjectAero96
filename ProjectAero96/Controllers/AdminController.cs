using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;
using ProjectAero96.Models;

namespace ProjectAero96.Controllers
{
    [EnumAuthorize(Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly IUserHelper userHelper;
        private readonly IAdminRepository adminRepository;

        public AdminController(IUserHelper userHelper, IAdminRepository adminRepository)
        {
            this.userHelper = userHelper;
            this.adminRepository = adminRepository;
        }

        // TODO fix on load visual bug (fixes itself on window resize for now)
        [Route("/admin")]
        public IActionResult Index()
        {
            return View();
        }

        // TODO controllers and views
        [Route("/admin/users")]
        public IActionResult Users()
        {
            return View();
        }

        [Route("/admin/airplanes")]
        public IActionResult Airplanes()
        {
            return View();
        }

        [Route("/admin/cities")]
        public IActionResult Cities()
        {
            return View();
        }

        [Route("/admin/users/edit")]
        public async Task<IActionResult> EditUser(string? userid)
        {
            if (string.IsNullOrEmpty(userid)) return NotFound();
            var model = await userHelper.FindUserByIdAsync(userid, true)
                                        .ToViewModelAsync(userHelper);
            if (model == null) return NotFound();
            return View(model);
        }

        [Route("/admin/users/get")]
        public async Task<JsonResult> GetUsers()
        {
            var users = await userHelper.GetUsersWithRoleAsync();
            return Json(new { users = users.ToViewModels(userHelper) });
        }
    }
}
