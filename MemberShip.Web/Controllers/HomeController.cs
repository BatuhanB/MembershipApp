using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MemberShip.Web.Extensions;

namespace MemberShip.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}