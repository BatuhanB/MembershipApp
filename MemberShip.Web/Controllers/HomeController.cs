using MemberShip.Web.Extensions;
using MemberShip.Web.Models;
using MemberShip.Web.Services;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MemberShip.Web.Controllers
{
    public class HomeController : Controller
    {
        #region DI Instances and Constructor

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

        #endregion

        #region Views

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

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["token"] = token;
            TempData["userId"] = userId;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

        #region SignUpAction

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = CreateUser(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Registration has completed completely!";
                return RedirectToAction("SignUp");
            }

            AddErrorsToModelStateForSignUp(result);
            return View();
        }

        private static AppUser CreateUser(SignUpViewModel model)
        {
            return new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
        }

        private void AddErrorsToModelStateForSignUp(IdentityResult result)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            ModelState.AddModelErrorList(errors);
        }

        #endregion

        #region SignInAction

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home");

            // Find user by email if it's not null return instance
            var userResult = FindUserByEmail(model.Email).Result.result;
            var user = FindUserByEmail(model.Email).Result.user;
            if (!userResult)
            {
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user!, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl!);
            }

            if (IfAccountLockedOut(result.IsLockedOut))
            {
                return View();
            }

            await AddErrorsToModelStateForSignIn(user);

            return View();
        }

        private async Task<(bool result, AppUser user)> FindUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found! - Please check your information!");
                return (false, user!);
            }
            return (true, user!);
        }

        private async Task AddErrorsToModelStateForSignIn(AppUser user)
        {
            var failedAttemptCount = await _userManager.GetAccessFailedCountAsync(user);
            ModelState.AddModelError(string.Empty, $"Login attempt have failed {failedAttemptCount} times!");
        }

        private bool IfAccountLockedOut(bool isLockedOut)
        {
            if (isLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Have to wait 1 minute the unlock fail attempt!");
                return true;
            }
            return false;
        }

        #endregion

        #region ForgetPasswordAction

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var userResult = FindUserByEmail(model.Email).Result.result;
            var user = FindUserByEmail(model.Email).Result.user;
            if (!userResult)
            {
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetEmail(passwordResetLink!, user.Email!);

            TempData["SuccessMessage"] = "Reset password link has been sent to your email";
            return RedirectToAction(nameof(ForgetPassword));
        }

        #endregion

        #region ResetPasswordAction

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var token = TempData["token"]?.ToString();
            var userId = TempData["userId"]?.ToString();

            IfTokenAndUserIdNull(token, userId);

            var hasUser = await _userManager.FindByIdAsync(userId!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "No user has found!");
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(hasUser, token!, model.Password);

            AddErrorsToModelStateForResetPassword(result);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been successfully reset";
                return View();
            }

            return View();
        }

        private void AddErrorsToModelStateForResetPassword(IdentityResult result)
        {
            var error = result.Errors.Select(x => x.Description).ToList();
            if (error.Any())
            {
                ModelState.AddModelErrorList(error);
            }
        }

        private static void IfTokenAndUserIdNull(string? token, string? userId)
        {
            if (userId == null || token == null)
            {
                throw new Exception("An error occured!");
            }
        }

        #endregion

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}