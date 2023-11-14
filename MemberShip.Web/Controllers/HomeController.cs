using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MemberShip.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View();
                }
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