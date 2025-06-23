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
            user = new User
            {
                Id = model.Id,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
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
        public async Task<IActionResult> EditUser(string uid, [Bind("Id","FirstName","LastName","Email","PhoneNumber","Address1","Address2","City","Country","Roles")]UserViewModel model)
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
        public async Task<IActionResult> CreateAirplane([Bind("Airline","Description","FCSeats","ESeats","AirlineImage","AirplaneModelId")]AirplaneViewModel model)
        {
            model.AirplaneModels = await adminRepository.GetAirplaneModelSelectItemsAsync();
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var airplaneModel = await adminRepository.GetAirplaneModelAsync(model.AirplaneModelId);
            if (airplaneModel == null)
            {
                ViewBag.Summary = FormSummary.Danger("Airplane model does not exist.");
                return View(model);
            }
            int seats = model.ESeats + model.FCSeats;
            if (seats == 0)
            {
                ViewBag.Summary = FormSummary.Danger("The total number of seats must not be zero.");
                return View(model);
            }
            if (seats > airplaneModel.MaxSeats)
            {
                ViewBag.Summary = FormSummary.Danger($"The total number of seats ({model.ESeats + model.FCSeats}) exceeds the maximum allowed for this airplane model ({airplaneModel.MaxSeats}).");
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
                Description = model.Description,
                FCSeats = model.FCSeats,
                ESeats = model.ESeats,
                AirlineImageId = model.AirlineImageId,
                AirplaneModelId = model.AirplaneModelId
            };
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
        public async Task<IActionResult> EditAirplane(int id, [Bind("Id","Airline","Description","FCSeats","ESeats","AirlineImageId","AirlineImage","AirplaneModelId","Deleted")]AirplaneViewModel model)
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
            var airplaneModel = await adminRepository.GetAirplaneModelAsync(model.AirplaneModelId);
            if (airplaneModel == null)
            {
                ViewBag.Summary = FormSummary.Danger("Airplane model does not exist.");
                return View(model);
            }
            int seats = model.ESeats + model.FCSeats;
            if (seats == 0)
            {
                ViewBag.Summary = FormSummary.Danger("The total number of seats must not be zero.");
                return View(model);
            }
            if (seats > airplaneModel.MaxSeats)
            {
                ViewBag.Summary = FormSummary.Danger($"The total number of seats ({model.ESeats + model.FCSeats}) exceeds the maximum allowed for this airplane model ({airplaneModel.MaxSeats}).");
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
            airplane.Description = model.Description;
            airplane.FCSeats = model.FCSeats;
            airplane.ESeats = model.ESeats;
            airplane.AirlineImageId = model.AirlineImageId;
            airplane.AirplaneModelId = model.AirplaneModelId;
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
            return View();
        }

        [Route("/admin/airplanemodels/create")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAirplaneModel([Bind("ModelName","ModelNameShort","PricePerTime","MaxSeats")] ModelAirplaneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["SummaryStyle"] = 3;
                TempData["Summary"] = "Something wrong happened.";
                return RedirectToAction("CreateAirplane");
            }
            var airplaneModel = new ModelAirplane
            {
                ModelName = model.ModelName,
                ModelNameShort = model.ModelNameShort,
                PricePerTime = model.PricePerTime,
                MaxSeats = model.MaxSeats
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
