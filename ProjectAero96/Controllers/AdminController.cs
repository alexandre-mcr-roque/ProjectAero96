using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly IImageHelper imageHelper;
        private readonly IMailHelper mailHelper;

        public AdminController(
            IUserHelper userHelper,
            IAdminRepository adminRepository,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            this.userHelper = userHelper;
            this.adminRepository = adminRepository;
            this.imageHelper = imageHelper;
            this.mailHelper = mailHelper;
        }

        // TODO fix on load visual bug (fixes itself on window resize for now)
        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            var user = await userHelper.FindUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.FullName = user.FullName;
            return View();
        }

        //================================================================
        // Users
        //================================================================
        [Route("/admin/users")]
        public IActionResult Users()
        {
            string? summary = (string?)TempData["Summary"];
            if (summary != null)
            {
                int style = (int)TempData["SummaryStyle"]!;
                ViewBag.Summary = FormSummary.FromCode(style, summary);
            }
            return View();
        }

        [Route("/admin/users/create")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [Route("/admin/users/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("FirstName","LastName","BirthDate","BypassAgeCheck","Email","PhoneNumber","Address1","Address2","City","Country","Roles")]UserViewModel model)
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
            if (model.BirthDate.Date > DateTime.UtcNow.AddYears(-18) || model.BypassAgeCheck)
            {
                ViewBag.Summary = FormSummary.Danger("User must be at least 18 years old.");
                return View(model);
            }
            user = new User
            {
                Id = model.Id,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = DateOnly.FromDateTime(model.BirthDate),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Country = model.Country,
                Deleted = model.Deleted
            };
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
            TempData["SummaryStyle"] = 2;
            TempData["Summary"] = "User created successfully. A password change email has been sent to the user.";
            return RedirectToAction("Users");
        }

        [Route("/admin/users/edit/{uid}")]
        public async Task<IActionResult> EditUser(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return NotFound();
            var model = await userHelper.FindUserByIdAsync(uid, true)
                                        .ToUserViewModelAsync();
            if (model == null) return NotFound();
            return View(model);
        }

        // TODO add permanent deletion option
        [Route("/admin/users/edit/{uid}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string uid, [Bind("Id","FirstName","LastName","BirthDate","BypassAgeCheck","Email","PhoneNumber","Address1","Address2","City","Country","Roles","Deleted")]UserViewModel model)
        {
            if (model.Id != uid) {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var user = await userHelper.FindUserByIdAsync(model.Id);
            if (user == null)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "User does not exist.";
                return RedirectToAction("Users");
            }

            if (model.BirthDate.Date > DateTime.UtcNow.AddYears(-18) || model.BypassAgeCheck)
            {
                ViewBag.Summary = FormSummary.Danger("User must be at least 18 years old.");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = DateOnly.FromDateTime(model.BirthDate);
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address1 = model.Address1;
            user.Address2 = model.Address2;
            user.City = model.City;
            user.Country = model.Country;
            user.Deleted = model.Deleted;

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
            return Json(new { users = users.ToUserViewModels() });
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
            var tokenLink = Url.Action("SetPassword", "Account", new
            {
                uid = user.Id,
                token = await userHelper.GenerateResetPasswordTokenAsync(user)
            }, protocol: HttpContext.Request.Scheme);
            string body = $"""
                <span style="font-size:2em">Change Password</span>
                <p>
                    To change your account's password, please click on the following link.
                    <br><br>
                    <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Change Password</a>
                </p>
                """;
            await mailHelper.SendEmailAsync(user.Email!, "Aero96 - Password Change", body);
        }

        //================================================================
        // Airplanes
        //================================================================
        [Route("/admin/airplanes")]
        public IActionResult Airplanes()
        {
            string? summary = (string?)TempData["Summary"];
            if (summary != null)
            {
                int style = (int)TempData["SummaryStyle"]!;
                ViewBag.Summary = FormSummary.FromCode(style, summary);
            }
            return View();
        }

        [Route("/admin/airplanes/create")]
        public async Task<IActionResult> CreateAirplane()
        {
            // TODO remove when airplane model index view is implemented
            string? summary = (string?)TempData["Summary"];
            if (summary != null)
            {
                int style = (int)TempData["SummaryStyle"]!;
                ViewBag.Summary = FormSummary.FromCode(style, summary);
            }
            var model = new AirplaneViewModel
            {
                AirplaneModels = await adminRepository.GetAirplaneModelSelectItemsAsync()
            };
            return View(model);
        }

        [Route("/admin/airplanes/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAirplane([Bind("Airline","Description","AirlineImage","AirplaneModelId","SeatRows","SeatColumns","WindowSeats")]AirplaneViewModel model)
        {
            model.AirplaneModels = await adminRepository.GetAirplaneModelSelectItemsAsync();
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            model.AirplaneModel = await adminRepository.GetAirplaneModelAsync(model.AirplaneModelId)
                                                       .ToModelAirplaneViewModelAsync();
            if (model.AirplaneModel == null)
            {
                ViewBag.Summary = FormSummary.Danger("Airplane model does not exist.");
                return View(model);
            }

            model.MaxSeats = model.AirplaneModel.MaxSeats;
            if (!ValidateSeatConfiguration(ModelState, model))
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            bool imageUploaded = true; // Set to true by default, in case no image is uploaded
            if (model.AirlineImage != null)
            {
                model.AirlineImageId = await imageHelper.UploadAirlineImageAsync(model.AirlineImage);
                imageUploaded = !string.IsNullOrEmpty(model.AirlineImageId);
            }
            var airplane = new Airplane
            {
                Id = model.Id,
                Airline = model.Airline,
                AirlineImageId = model.AirlineImageId,
                AirplaneModelId = model.AirplaneModelId,
                MaxSeats = model.MaxSeats,
                SeatRows = model.SeatRows,
                SeatColumns = model.SeatColumns,
                WindowSeats = model.WindowSeats,
            };
            if (string.IsNullOrEmpty(model.Description)) airplane.Description = model.AirplaneModel.ToString();
            else airplane.Description = model.Description;
            var result = await adminRepository.AddAirplaneAsync(airplane);
            if (!result)
            {
                if (imageUploaded)
                {
                    await imageHelper.DeleteAirlineImageAsync(model.AirlineImageId!);
                }
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }

            if (imageUploaded)
            {
                TempData["SummaryStyle"] = 2;
                TempData["Summary"] = "Airplane edited successfully.";
            }
            else
            {
                TempData["SummaryStyle"] = 4;
                TempData["Summary"] = "Airplane edited successfully, but the image was not uploaded.";
            }
            //if (imageUploaded)  ViewBag.Summary = FormSummary.Success("Airplane created successfully.");
            //else ViewBag.Summary = FormSummary.Warning("Airplane created successfully, but the image was not uploaded.");
            return RedirectToAction("Airplanes");
        }

        [Route("/admin/airplanes/edit/{id:int}")]
        public async Task<IActionResult> EditAirplane(int id)
        {
            var model = await adminRepository.GetAirplaneAsync(id)
                                             .ToAirplaneViewModelAsync();
            if (model == null) return NotFound();
            model.AirplaneModels = await adminRepository.GetAirplaneModelSelectItemsAsync();
            return View(model);
        }

        [Route("/admin/airplanes/edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAirplane(int id, [Bind("Id","Airline","Description","AirlineImageId","AirlineImage","AirplaneModelId","SeatRows","SeatColumns","WindowSeats","Deleted")]AirplaneViewModel model)
        {
            if (model.Id != id)
            {
                return NotFound();
            }
            model.AirplaneModels = await adminRepository.GetAirplaneModelSelectItemsAsync();
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var airplane = await adminRepository.GetAirplaneAsync(model.Id);
            if (airplane == null)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Airplane does not exist.";
                return RedirectToAction("Airplanes");
            }
            if (model.Deleted && await adminRepository.IsAirplaneInUse(airplane))
            {
                ViewBag.Summary = FormSummary.Danger("Airplane is currently in use and cannot be disabled");
                model.Deleted = false; // Reset the Deleted property to false
                return View(model);
            }

            model.AirplaneModel = await adminRepository.GetAirplaneModelAsync(model.AirplaneModelId)
                                                       .ToModelAirplaneViewModelAsync();
            if (model.AirplaneModel == null)
            {
                ViewBag.Summary = FormSummary.Danger("Airplane model does not exist.");
                return View(model);
            }

            model.MaxSeats = model.AirplaneModel.MaxSeats;
            if (!ValidateSeatConfiguration(ModelState, model))
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }

            bool imageUploaded = true; // Set to true by default, in case no image is uploaded
            if (model.AirlineImage != null)
            {
                var imageId = await imageHelper.UploadAirlineImageAsync(model.AirlineImage, model.AirlineImageId);
                imageUploaded = !string.IsNullOrEmpty(imageId);
                if (imageUploaded) model.AirlineImageId = imageId;
            }

            airplane.Airline = model.Airline;
            if (string.IsNullOrEmpty(model.Description)) airplane.Description = model.AirplaneModel.ToString();
            else airplane.Description = model.Description;
            airplane.AirlineImageId = model.AirlineImageId;
            airplane.AirplaneModelId = model.AirplaneModelId;
            airplane.MaxSeats = model.MaxSeats;
            airplane.SeatRows = model.SeatRows;
            airplane.SeatColumns = model.SeatColumns;
            airplane.WindowSeats = model.WindowSeats;
            airplane.Deleted = model.Deleted;
            var result = await adminRepository.UpdateAirplaneAsync(airplane);
            if (!result)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }

            //if (imageUploaded)
            //{
            //    TempData["SummaryStyle"] = 2;
            //    TempData["Summary"] = "Airplane edited successfully.";
            //}
            //else
            //{
            //    TempData["SummaryStyle"] = 4;
            //    TempData["Summary"] = "Airplane edited successfully, but the image was not uploaded.";
            //}
            if (imageUploaded) ViewBag.Summary = FormSummary.Success("Airplane edited successfully.");
            else ViewBag.Summary = FormSummary.Warning("Airplane edited successfully, but the image was not uploaded.");
            return View(model);
        }

        [Route("/admin/airplanes/getall")]
        public async Task<JsonResult> GetAirplanes()
        {
            var airplanes = await adminRepository.GetAirplanesAsync();
            return Json(new { airplanes = airplanes.ToAirplaneViewModels() });
        }

        [Route("/admin/airplanes/disable/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableAirplane(int id)
        {
            var airplane = await adminRepository.GetAirplaneAsync(id);
            if (airplane == null)
            {
                return NotFound("Airplane does not exist");
            }
            if (await adminRepository.IsAirplaneInUse(airplane))
            {
                return NotFound("Airplane is currently in use and cannot be disabled");
            }
            if (airplane.Deleted)
            {
                return NotFound("Airplane is already disabled");
            }
            airplane.Deleted = true;
            var result = await adminRepository.UpdateAirplaneAsync(airplane);
            if (!result)
            {
                return NotFound("Unknown error");
            }
            return Ok(airplane);
        }

        [Route("/admin/airplanes/restore/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreAirplane(int id)
        {
            var airplane = await adminRepository.GetAirplaneAsync(id);
            if (airplane == null)
            {
                return NotFound("Airplane does not exist");
            }
            if (!airplane.Deleted)
            {
                return NotFound("Airplane is not disabled");
            }
            airplane.Deleted = false;
            var result = await adminRepository.UpdateAirplaneAsync(airplane);
            if (!result)
            {
                return NotFound("Unknown error");
            }
            return Ok(airplane);
        }

        //================================================================
        // Airplane Models
        //================================================================
        // TODO add index, edit and delete options, and change redirection in post action and view
        [Route("/admin/airplanemodels/create")]
        public IActionResult CreateAirplaneModel()
        {
            return View(new ModelAirplaneViewModel());
        }

        [Route("/admin/airplanemodels/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAirplaneModel([Bind("ModelName","ModelNameShort","PricePerHour","MaxSeats","SeatRows","SeatColumns","WindowSeats")] ModelAirplaneViewModel model)
        {
            if (!ModelState.IsValid || !ValidateSeatConfiguration(ModelState, model))
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Something wrong happened.";
                return RedirectToAction("CreateAirplane");
            }
            var airplaneModel = new ModelAirplane
            {
                ModelName = model.ModelName,
                ModelNameShort = model.ModelNameShort,
                PricePerHour = model.PricePerHour,
                MaxSeats = model.MaxSeats,
                SeatRows = model.SeatRows,
                SeatColumns = model.SeatColumns,
                WindowSeats = model.WindowSeats
            };
            var result = await adminRepository.AddAirplaneModelAsync(airplaneModel);
            if (!result)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Something wrong happened. Please try again later.";
                return RedirectToAction("CreateAirplane");
            }
            TempData["SummaryStyle"] = 2;
            TempData["Summary"] = "Airplane model created successfully.";
            return RedirectToAction("CreateAirplane");
        }

        private bool ValidateSeatConfiguration(ModelStateDictionary modelState, ISeatConfigurationModel model)
        {
            if (model.SeatRows * model.SeatColumns > model.MaxSeats)
            {
                modelState.AddModelError("SeatRows", "Number of seats exceed the maximum amount of seats.");
                modelState.AddModelError("SeatColumns", "Number of seats exceed the maximum amount of seats.");
                return false;
            }
            if (model.SeatColumns == 1)
            {
                if (model.WindowSeats != 1)
                {
                    modelState.AddModelError("WindowSeats", "This configuration of window seats is invalid.");
                    return false;
                }
            }
            else
            {
                int centerSeats = model.SeatColumns - model.WindowSeats * 2;
                if (centerSeats < 0 || centerSeats > 4)
                {
                    modelState.AddModelError("WindowSeats", "This configuration of window seats is invalid.");
                    return false;
                }
            }
            return true;
        }

        [Route("/admin/airplanemodels/seat-config/{id}")]
        public async Task<JsonResult> GetAirplaneModelSeatConfig(int id)
        {
            var airplaneModel = await adminRepository.GetAirplaneModelAsync(id);
            if (airplaneModel == null)
            {
                return Json(0);
            }
            return Json(new { airplaneModel.MaxSeats, airplaneModel.SeatRows, airplaneModel.SeatColumns, airplaneModel.WindowSeats });
        }

        //================================================================
        // Cities
        //================================================================
        [Route("/admin/cities")]
        public IActionResult Cities()
        {
            string? summary = (string?)TempData["Summary"];
            if (summary != null)
            {
                int style = (int)TempData["SummaryStyle"]!;
                ViewBag.Summary = FormSummary.FromCode(style, summary);
            }
            return View();
        }

        [Route("/admin/cities/create")]
        public IActionResult CreateCity()
        {
            return View();
        }

        [Route("/admin/cities/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity([Bind("Name","Country")]CityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var city = new City
            {
                Id = model.Id,
                Name = model.Name,
                Country = model.Country
            };
            var result = await adminRepository.AddCityAsync(city);
            if (!result)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            TempData["SummaryStyle"] = 2;
            TempData["Summary"] = "City created successfully.";
            return RedirectToAction("Cities");
        }

        [Route("/admin/cities/edit/{id:int}")]
        public async Task<IActionResult> EditCity(int id)
        {
            var model = await adminRepository.GetCityAsync(id)
                                             .ToCityViewModelAsync();
            if (model == null) return NotFound();
            return View(model);
        }

        [Route("/admin/cities/edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, [Bind("Id","Name","Country","Deleted")]CityViewModel model)
        {
            if (model.Id != id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var city = await adminRepository.GetCityAsync(model.Id);
            if (city == null)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "City does not exist.";
                return RedirectToAction("Cities");
            }
            if (model.Deleted && await adminRepository.IsCityInUse(city))
            {
                ViewBag.Summary = FormSummary.Danger("City is currently in use and cannot be disabled");
                model.Deleted = false; // Reset the Deleted property to false
                return View(model);
            }
            city.Name = model.Name;
            city.Country = model.Country;
            city.Deleted = model.Deleted;
            var result = await adminRepository.UpdateCityAsync(city);
            if (!result)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            ViewBag.Summary = FormSummary.Success("City edited successfully.");
            return View(model);
        }

        [Route("/admin/cities/getall")]
        public async Task<JsonResult> GetCities()
        {

            var cities = await adminRepository.GetCitiesAsync();
            return Json(new { cities = cities.ToCityViewModels() });
        }

        [Route("/admin/cities/disable/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableCity(int id)
        {
            var city = await adminRepository.GetCityAsync(id);
            if (city == null)
            {
                return NotFound("City does not exist");
            }
            if (await adminRepository.IsCityInUse(city))
            {
                return NotFound("City is currently in use and cannot be disabled");
            }
            if (city.Deleted)
            {
                return NotFound("City is already disabled");
            }
            city.Deleted = true;
            var result = await adminRepository.UpdateCityAsync(city);
            if (!result)
            {
                return NotFound("Unknown error");
            }
            return Ok(city);
        }

        [Route("/admin/cities/restore/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCity(int id)
        {
            var city = await adminRepository.GetCityAsync(id);
            if (city == null)
            {
                return NotFound("City does not exist");
            }
            if (!city.Deleted)
            {
                return NotFound("City is not disabled");
            }
            city.Deleted = false;
            var result = await adminRepository.UpdateCityAsync(city);
            if (!result)
            {
                return NotFound("Unknown error");
            }
            return Ok(city);
        }
    }
}
