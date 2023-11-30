using MemberShip.Web.Areas.Admin.Models;
using MemberShip.Web.Extensions;
using MemberShip.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemberShip.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRoles> _roleManager;

        public RolesController(UserManager<AppUser> userManager,
            RoleManager<AppRoles> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleListViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();
            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            var result = await _roleManager.CreateAsync(new() { Name = model.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            TempData["SuccessMessage"] = "Role have been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new Exception("No role has found!");
            }

            return View(new UpdateRoleViewModel() { Id = id, Name = role.Name! });
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            role!.Name = model.Name;
            await _roleManager.UpdateAsync(role);
            TempData["SuccessMessage"] = "Role have been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new Exception("No role has found!");
            }
            var result = await _roleManager.DeleteAsync(role);

            if(!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x=>x.Description).First());
            }

            TempData["SuccessMessage"] = "Role have been deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

    }
}
