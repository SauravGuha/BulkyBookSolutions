using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = nameof(Roles.SuperAdmin))]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public IActionResult CreateEdit(string id)
        {
            //Add
            if (id == null)
            {
                return View(new IdentityRole());
            }
            else
            {
                var role = _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    TempData["Error"] = $"No cover found with id={id}.";
                }
                else
                {
                    return View(role);
                }
            }
            return View();
        }

    }
}
