using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MemberShip.Web.Extensions;
using MemberShip.Web.Services;
using NuGet.Common;

namespace MemberShip.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration has completed completely!";
                    return RedirectToAction("SignUp");
                }
                var errors = result.Errors.Select(e => e.Description).ToList();
                ModelState.AddModelErrorList(errors);
                return View();
            }
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home");
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Please check your information!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user!, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl!);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Have to wait 1 minute the unlock fail attempt!");
                return View();
            }
            var failedAttemptCount = await _userManager.GetAccessFailedCountAsync(user);
            ModelState.AddModelError(string.Empty, $"Login attempt have failed {failedAttemptCount} times!");
            return View();
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasUser = await _userManager.FindByEmailAsync(model.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, $" {model.Email} user has not found");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Reset password link has been sent to your email";
            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["token"] = token;
            TempData["userId"] = userId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var token = TempData["token"];
            var userId = TempData["userId"];

            if(userId == null || token == null)
            {
                throw new Exception("An error occured!");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "No user has found!");
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, model.Password);

            if(result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been successfully reset";
                return View();
            }
            var error = result.Errors.Select(x=>x.Description).ToList();
            if (error.Any())
            {
                ModelState.AddModelErrorList(error);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}