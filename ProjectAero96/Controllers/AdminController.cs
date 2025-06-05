using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;

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
        [Route("admin")]
        public async Task<IActionResult> Index()
        {
            var user = await userHelper.FindUserByEmailAsync(User.Identity!.Name!);
            ViewBag.FullName = user!.FullName;
            return View();
        }

        // TODO controllers and views
        [Route("admin/users")]
        public IActionResult Users()
        {
            return View();
        }

        [Route("admin/airplanes")]
        public IActionResult Airplanes()
        {
            return View();
        }

        [Route("admin/cities")]
        public IActionResult Cities()
        {
            return View();
        }

        // will be used to asyncronously get a page of users for datatable listing
        public async Task<JsonResult> GetUsers(int? page = 1, int? size = 20)
        {
            --page; // switch to 0-index paging
            return Json(new { users = await userHelper.GetUsersAsync(page!.Value,size!.Value) });
        }
    }
}
