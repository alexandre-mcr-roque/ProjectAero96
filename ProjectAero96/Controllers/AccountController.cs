﻿using Microsoft.AspNetCore.Authorization;
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

            user = model.ToEntity();
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
            ViewBag.Summary = FormSummary.Success("An email has been sent to your inbox to complete the registration.");  // Repurposed for success messages
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        [Route("/accountverified")]
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
            await mailHelper.SendEmailAsync(user.Email!, "Email Verification", body);
        }

        [Route("/account/changepassword")]
        public async Task<IActionResult> ChangePassword(string? uid, string? token)
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

        [Route("/account/changepassword")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string? uid, string? token,
            [Bind("Password","ConfirmPassword")]ChangePasswordModel model)
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

            return View();
        }

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
    }
}
