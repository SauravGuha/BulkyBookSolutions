using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBook.Web.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel();
            var products = this._unitOfWork.ProductRepository.GetAll(nameof(Product.Category), nameof(Product.CoverType));
            homeViewModel.Products = products.Select(e => new ProductViewModel()
            {
                Product = e
            });
            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            var productDetails = this._unitOfWork.ProductRepository.GetByExpression(e => e.Id == id,
nameof(Product.Category), nameof(Product.CoverType));
            var detailsViewModel = new DetailsViewModel()
            {
                Count = 0,
                ShoppingCartId = 0,
                OldCount = 0
            };
            detailsViewModel.Product = productDetails;
            if (HttpContext.User.Claims.Count() > 0)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var shoppingCart = this._unitOfWork.ShoppingCartRepository.GetByExpression(e => e.ProductId == id && e.UserId == user.Id);
                if (shoppingCart != null)
                {
                    detailsViewModel.Count = shoppingCart.Count;
                    detailsViewModel.ShoppingCartId = shoppingCart.Id;
                    detailsViewModel.OldCount = shoppingCart.Count;
                }
            }

            return View(detailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(DetailsViewModel detailsViewModel)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var shoppingCartModel = new ShoppingCart()
                {
                    Count = detailsViewModel.Count,
                    ProductId = detailsViewModel.Product.Id,
                    UserId = user.Id,
                };
                if (detailsViewModel.ShoppingCartId != 0)
                {
                    shoppingCartModel.Id = detailsViewModel.ShoppingCartId;
                    shoppingCartModel.Count = detailsViewModel.OldCount + detailsViewModel.Count;
                    this._unitOfWork.ShoppingCartRepository.Update(shoppingCartModel);
                }
                else
                    this._unitOfWork.ShoppingCartRepository.Insert(shoppingCartModel);
                this._unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}