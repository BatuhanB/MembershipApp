using MemberShip.Web.Extensions;
using MemberShip.Web.Models;
using MemberShip.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MemberShip.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ProfileController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var userProfile = new UserProfileViewModel()
            {
                Email = currentUser!.Email!,
                Name = currentUser.UserName!,
                PhoneNumber = currentUser.PhoneNumber
            };
            return View(userProfile);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserPasswordChangeViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (!ModelState.IsValid)
            {
                return View();
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user!, model.OldPassword);
            if (!checkPassword)
            {
                ModelState.AddModelError(string.Empty, "Current Password is not correct");
                return View();
            }

            var result = await _userManager.ChangePasswordAsync(user!, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var err = result.Errors.Select(x => x.Description).ToList();
                ModelState.AddModelErrorList(err);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user!);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user!, model.NewPassword, true, false);
            TempData["SuccessMessage"] = "Password has been changed successfully!";

            return View();
        }


        //private async Task<IActionResult> CheckIfPasswordCorrect(string password)
        //{
            
        //}
    }
}
