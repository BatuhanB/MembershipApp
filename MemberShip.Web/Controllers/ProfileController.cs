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
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = FindUserByName(User.Identity!.Name!).Result.result;
            var user = FindUserByName(User.Identity!.Name!).Result.user;
            if (!result)
            {
                return View();
            }

            if (!await CheckOldPassword(user!, model.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "Current Password is not correct");
                return View();
            }

            if (!await ChangePassword(user, model.OldPassword, model.NewPassword))
            {
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user!);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user!, model.NewPassword, true, false);
            TempData["SuccessMessage"] = "Password has been changed successfully!";

            return View();
        }

        private async Task<bool> CheckOldPassword(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<bool> ChangePassword(AppUser user, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user!, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                var err = result.Errors.Select(x => x.Description).ToList();
                ModelState.AddModelErrorList(err);
                return false;
            }
            return true;
        }

        private async Task<(bool result, AppUser user)> FindUserByName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found! - Please check your information!");
                return (false, user!);
            }
            return (true, user!);
        }
    }
}
