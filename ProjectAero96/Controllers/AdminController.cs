using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Entities;
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
        private readonly IMailHelper mailHelper;

        public AdminController(
            IUserHelper userHelper,
            IAdminRepository adminRepository,
            IMailHelper mailHelper)
        {
            this.userHelper = userHelper;
            this.adminRepository = adminRepository;
            this.mailHelper = mailHelper;
        }

        // TODO fix on load visual bug (fixes itself on window resize for now)
        [Route("/admin")]
        public IActionResult Index()
        {
            return View();
        }

        // TODO controllers and views
        //================================================================
        // Users
        //================================================================
        [Route("/admin/users")]
        public IActionResult Users()
        {
            return View();
        }

        [Route("/admin/users/create")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [Route("/admin/users/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("FirstName","LastName","Email","PhoneNumber","Address1","Address2","City","Country","Roles")]UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var user = await userHelper.FindUserByEmailAsync(model.Email);
            if (user != null)
            {
                ViewBag.Summary = FormSummary.Danger("There is already a registered account with the given email.");
                return View(model);
            }
            user = await model.ToNewEntityAsync(userHelper);
            // Automatically confirm email for admin-created users
            // (they still need to change the password before signing in, thus indirectly confirming their email)
            user.EmailConfirmed = true;
            var result = await userHelper.AddUserAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }

            await SendPasswordChangeEmail(user);
            ViewBag.Summary = FormSummary.Success("User created successfully.");
            return View(model);
        }

        [Route("/admin/users/edit/{uid}")]
        public async Task<IActionResult> EditUser(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return NotFound();
            var model = await userHelper.FindUserByIdAsync(uid, true)
                                        .ToViewModelAsync();
            if (model == null) return NotFound();
            return View(model);
        }

        [Route("/admin/users/edit/{uid}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string uid, [Bind("FirstName", "LastName", "Email", "PhoneNumber", "Address1", "Address2", "City", "Country", "Roles")] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var user = await userHelper.FindUserByEmailAsync(model.Email);
            if (user == null || user.Id != uid)
            {
                return RedirectToAction("Users");
            }
            
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address1 = model.Address1;
            user.Address2 = model.Address2;
            user.City = model.City;
            user.Country = model.Country;

            user.Roles.Clear();
            if (model.IsAdmin)
            {
                var role = await userHelper.GetRolesAsync(Roles.Admin);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            if (model.IsEmployee)
            {
                var role = await userHelper.GetRolesAsync(Roles.Employee);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            if (model.IsClient)
            {
                var role = await userHelper.GetRolesAsync(Roles.Client);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            var result = await userHelper.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }

            ViewBag.Summary = FormSummary.Success("User edited successfully.");
            return View(model);
        }

        [Route("/admin/users/getall")]
        public async Task<JsonResult> GetUsers()
        {
            var users = await userHelper.GetUsersWithRoleAsync();
            return Json(new { users = users.ToViewModels() });
        }

        [Route("/admin/users/disable/{uid}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableUser(string uid)
        {
            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            if (user.Deleted)
            {
                return NotFound("User is already disabled");
            }
            if (user.UserName == User.Identity!.Name)
            {
                return Unauthorized("Unable to perform action on itself");
            }
            var result = await userHelper.SetUserDeleted(user);
            if (!result.Succeeded)
            {
                return NotFound("Unknown error");
            }
            return Ok(user);
        }

        [Route("/admin/users/restore/{uid}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreUser(string uid)
        {
            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            if (!user.Deleted)
            {
                return NotFound("User is not disabled");
            }
            if (user.UserName == User.Identity!.Name)
            {
                return Unauthorized("Unable to perform action on itself");
            }
            var result = await userHelper.SetUserDeleted(user, false);
            if (!result.Succeeded)
            {
                return NotFound("Unknown error");
            }
            return Ok(user);
        }

        // TODO maybe change text to something more descriptive since this is a new account
        public async Task SendPasswordChangeEmail(User user)
        {
            var tokenLink = Url.Action("ChangePassword", "Account", new
            {
                uid = user.Id,
                token = await userHelper.GenerateChangePasswordTokenAsync(user)
            }, protocol: HttpContext.Request.Scheme);
            string body = $"""
                <span style="font-size:2em">Change Password</span>
                <p>
                    To change your account's password, please click on the following link.
                    <br><br>
                    <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Change Password</a>
                </p>
                """;
            await mailHelper.SendEmailAsync(user.Email!, "Password Change", body);
        }

        //================================================================
        // Airplanes
        //================================================================
        [Route("/admin/airplanes")]
        public IActionResult Airplanes()
        {
            return View();
        }

        //================================================================
        // Cities
        //================================================================
        [Route("/admin/cities")]
        public IActionResult Cities()
        {
            return View();
        }
    }
}
