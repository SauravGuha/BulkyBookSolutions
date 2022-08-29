using BulkyBook.Common;
using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace BulkyBook.Web.Components
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<Customer> _userManager;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                if (!HttpContext.Session.Keys.Any(e => e == Constants.CartCount))
                {
                    var userCartCount = this._unitOfWork.ShoppingCartRepository.GetAll(e => e.UserId == user.Id).Count();
                    HttpContext.Session.SetInt32(Constants.CartCount, userCartCount);
                }
                var cartCount = HttpContext.Session.GetInt32(Constants.CartCount);
                return View(new Dummy() { CartCount = cartCount?.ToString() });
            }

            return View(new Dummy());
        }

    }

    public class Dummy
    {
        public string CartCount { get; set; } = "0";
    }
}
