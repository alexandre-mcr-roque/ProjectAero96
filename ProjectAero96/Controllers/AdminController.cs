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

        // TODO make view
        [Route("admin")]
        public IActionResult Index()
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
