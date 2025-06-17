using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;
using ProjectAero96.Models;

namespace ProjectAero96.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper userHelper;
        private readonly IMailHelper mailHelper;

        public AccountController(
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            this.userHelper = userHelper;
            this.mailHelper = mailHelper;
        }

        [Route("/signout"), Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await userHelper.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("/signin")]
        public IActionResult SignIn()
        {
            return View();
        }

        [Route("/signin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([Bind("Email","Password","RememberMe")]SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                model.Password = string.Empty;
                return View(model);
            }

            var user = await userHelper.FindUserByEmailAsync(model.Email);
            if (user == null || user.Deleted)
            {
                ViewBag.Summary = FormSummary.Danger("There is no account registered with the email provided.");
                model.Password = string.Empty;
                return View(model);
            }

            if (user.RequiresPasswordChange)             {
                ViewBag.Summary = FormSummary.Danger("You must change your password before signing in.");
                model.Password = string.Empty;
                return View(model);
            }

            var result = await userHelper.SignInAsync(user, model.Password, model.RememberMe);
            if (!result.Succeeded)
            {
                ViewBag.Summary = FormSummary.Danger("Invalid credentials.");
                model.Password = string.Empty;
                return View(model);
            }

            if (Request.Query.Keys.Contains("ReturnUrl"))
            {
                return Redirect(Request.Query["ReturnUrl"]!);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // TODO cleanup view
        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        // TODO add way to resend verification token
        [Route("/register")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName","LastName","Email","PhoneNumber","Address1","Address2","City","Country","Password","ConfirmPassword")]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            var user = await userHelper.FindUserByEmailAsync(model.Email);
            if (user != null)
            {
                ViewBag.Summary = FormSummary.Danger("There is already a registered account with the given email.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            user = model.ToNewEntity();
            var result = await userHelper.AddUserAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }
            await userHelper.AddUserToRoleAsync(user, Roles.Client);

            await SendVerificationEmail(user);
            ViewBag.Summary = FormSummary.Success("An email has been sent to your inbox to complete the registration."); // Repurposed for success messages
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        [Authorize]
        [Route("/account/information")]
        public async Task<IActionResult> Information()
        {
            var model = await userHelper.FindUserByEmailAsync(User.Identity!.Name!)
                                        .ToAccountViewModelAsync();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [Authorize]
        [Route("/account/information/edit")]
        public async Task<IActionResult> EditInformation()
        {
            var model = await userHelper.FindUserByEmailAsync(User.Identity!.Name!)
                                        .ToAccountViewModelAsync();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }


        [Authorize]
        [Route("/account/information/edit")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInformation([Bind("FirstName","LastName","PhoneNumber","Address1","Address2","City","Country")]AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View(model);
            }
            var user = await userHelper.FindUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address1 = model.Address1;
            user.Address2 = model.Address2;
            user.City = model.City;
            user.Country = model.Country;
            var result = await userHelper.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened. Please try again later.");
                return View(model);
            }
            ViewBag.Summary = FormSummary.Success("Information updated successfully.");
            return RedirectToAction("Information");
        }

        [Authorize]
        [Route("/account/information/changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [Route("/account/information/changepassword")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind("Password","NewPassword","ConfirmPassword")]ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                return View();
            }
            var user = await userHelper.FindUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var result = await userHelper.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Password", "The password is incorrect.");
                return View();
            }
            ViewBag.Summary = FormSummary.Success("Password changed successfully.");
            return RedirectToAction("Information");
        }

        [Route("/account/verified")]
        public async Task<IActionResult> VerifyEmail(string? uid, string? token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userHelper.VerifyEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public async Task SendVerificationEmail(User user)
        {
            var tokenLink = Url.Action("VerifyEmail", "Account", new
            {
                uid = user.Id,
                token = await userHelper.GenerateVerifyEmailTokenAsync(user)
            }, protocol: HttpContext.Request.Scheme);
            string body = $"""
                <span style="font-size:2em">Verify Email</span>
                <p>
                    To complete the account registration, please click on the following link.
                    <br><br>
                    <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Verify Registration</a>
                </p>
                """;
            await mailHelper.SendEmailAsync(user.Email!, "Aero96 - Email Verification", body);
        }

        [Route("/account/setpassword")]
        public async Task<IActionResult> SetPassword(string? uid, string? token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null || !user.RequiresPasswordChange)
            {
                return NotFound();
            }

            var result = await userHelper.VerifyEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        [Route("/account/setpassword")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(string? uid, string? token,
            [Bind("Password","ConfirmPassword")]SetPasswordModel model)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            ViewBag.Summary = FormSummary.Success("Password reset successfully. You can now sign in with your new password.");
            return RedirectToAction("SignIn");
        }

        [Route("/account/resetpassword")]
        public async Task<IActionResult> ResetPassword(string? uid, string? token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userHelper.VerifyEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        [Route("/account/resetpassword")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string? uid, string? token,
            [Bind("Password", "ConfirmPassword")] SetPasswordModel model)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.FindUserByIdAsync(uid);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Summary = FormSummary.Danger("Something wrong happened.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            ViewBag.Summary = FormSummary.Success("Password reset successfully. You can now sign in with your new password.");
            return RedirectToAction("SignIn");
        }

        public async Task SendPasswordResetEmail(User user)
        {
            var tokenLink = Url.Action("ResetPassword", "Account", new
            {
                uid = user.Id,
                token = await userHelper.GenerateResetPasswordTokenAsync(user)
            }, protocol: HttpContext.Request.Scheme);
            string body = $"""
                <span style="font-size:2em">Reset Password</span>
                <p>
                    To reset your account's password, please click on the following link.
                    <br><br>
                    <a style="background-color:#0d6efd;padding:.375em .75em;border-radius:.25em;color:#fff;text-decoration:none;border:1px solid #0d6efd" href="{tokenLink}">Reset Password</a>
                </p>
                """;
            await mailHelper.SendEmailAsync(user.Email!, "Aero96 - Reset Password", body);
        }
    }
}
