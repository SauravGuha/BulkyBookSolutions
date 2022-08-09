using BulkyBook.Common.Interfaces;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Authorize(Roles = nameof(Roles.SuperAdmin))]
    [Area("Admin")]
    public class UserController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<Common.Models.Customer> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<Common.Models.Customer> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var nonLoggedInUsers = _userManager.Users.ToList();
            if (currentUser != null)
                nonLoggedInUsers = _userManager.Users.Where(e => e.Name != currentUser.Name).ToList();
            return View(nonLoggedInUsers);
        }

        public async Task<IActionResult> RoleList(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var otherRoles = this._roleManager.Roles;

            var userRoleList = new List<UserRoleViewModel>();
            foreach (var role in currentUserRoles)
            {
                userRoleList.Add(new UserRoleViewModel
                {
                    IsAssigned = true,
                    RoleName = role

                });
            }
            foreach (var role in otherRoles)
            {
                if (!userRoleList.Any(e => e.RoleName == role.Name))
                {
                    userRoleList.Add(new UserRoleViewModel
                    {
                        IsAssigned = false,
                        RoleName = role.Name

                    });
                }
            }
            var manageUserRole = new ManageUserRoleViewModel()
            {
                UserRoleId = id,
                Roles = userRoleList
            };
            return View(manageUserRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(ManageUserRoleViewModel manageUserRoleViewModel)
        {
            var user = await this._userManager.FindByIdAsync(manageUserRoleViewModel.UserRoleId);
            if (user != null)
            {
                var userRoles = await this._userManager.GetRolesAsync(user);
                await this._userManager.RemoveFromRolesAsync(user, userRoles);
                await this._userManager.AddToRolesAsync(user, manageUserRoleViewModel.Roles.Where(e => e.IsAssigned).Select(e => e.RoleName));
                TempData["Success"] = "Successfully updated roles.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "No user found.";
                return View(nameof(RoleList), manageUserRoleViewModel);
            }
        }
    }
}
