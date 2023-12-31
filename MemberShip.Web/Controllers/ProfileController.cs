﻿using MemberShip.Web.Extensions;
using MemberShip.Web.Models;
using MemberShip.Web.Models.Enums;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            //ViewBag.GenderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var userProfile = new UserProfileViewModel()
            {
                Email = currentUser!.Email!,
                Name = currentUser.UserName!,
                PhoneNumber = currentUser.PhoneNumber!,
                City = currentUser.City,
                BirthDate = currentUser.BirthDate,
                Gender = currentUser.Gender,
                ShowPicture = currentUser.Picture
            };
            return View(userProfile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);

            user.UserName = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.BirthDate = model.BirthDate;
            user.Gender = (Gender)model!.Gender!;

            if (model.Picture != null && model.Picture.Length > 0)
            {
                // TODO: When update profile birthdate does not shows in input
                var wwwrootFolder = _webHostEnvironment.ContentRootPath;
                var imgDirectory = Path.Combine(wwwrootFolder, "img", "user-profile-pictures");
                var fileName = $"{model.Picture.FileName}{Path.GetExtension(model.Picture.FileName)}";

                var newPath = Path.Combine(imgDirectory, fileName);

                if (System.IO.File.Exists(newPath))
                {
                    fileName = Guid.NewGuid().ToString() + fileName;
                }

                using var stream = new FileStream(newPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024);
                await model.Picture.CopyToAsync(stream);
                await stream.FlushAsync();
                user.Picture = fileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            var userProfile = new UserProfileViewModel()
            {
                Email = user!.Email!,
                Name = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                City = user.City,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                ShowPicture = user.Picture
            };

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);
            TempData["SuccessMessage"] = "User credentials has been changed successfully!";
            return View("Index", userProfile);
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
