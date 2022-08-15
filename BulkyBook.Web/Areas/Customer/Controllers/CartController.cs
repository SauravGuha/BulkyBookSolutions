using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private UserManager<Common.Models.Customer> _userManager;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;

        public CartController(UserManager<Common.Models.Customer> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
            this._configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var user = await this._userManager.GetUserAsync(HttpContext.User);

            var shoppingCartViewModel = this.GetShoppingCartViewModel(user);

            return View(shoppingCartViewModel);
        }

        public IActionResult DeleteCart(int? id)
        {
            var shoppingCart = this._unitOfWork.ShoppingCartRepository.Find(id);
            if (shoppingCart != null)
            {
                this._unitOfWork.ShoppingCartRepository.Delete(shoppingCart);
                this._unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddQuantityToCart(int? id)
        {
            var shoppingCart = this._unitOfWork.ShoppingCartRepository.Find(id);
            if (shoppingCart != null)
            {
                shoppingCart.Count++;
                this._unitOfWork.ShoppingCartRepository.Update(shoppingCart);
                this._unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeductQuantityFromCart(int? id)
        {
            var shoppingCart = this._unitOfWork.ShoppingCartRepository.Find(id);
            if (shoppingCart != null)
            {
                shoppingCart.Count--;
                this._unitOfWork.ShoppingCartRepository.Update(shoppingCart);
                this._unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            var summaryViewModel = new OrderSummaryViewModel();
            var user = await this._userManager.GetUserAsync(HttpContext.User);

            var addresses = this._unitOfWork.AddressRepository.GetAll(e => e.CustomerId == user.Id).ToList();
            summaryViewModel.Addresses = this._unitOfWork.AddressRepository.GetAll(e => e.CustomerId == user.Id).ToList();
            summaryViewModel.SelectAddressItem = addresses.Select(e => new SelectListItem()
            {
                Text = e.AddressLine1,
                Value = e.Id.ToString()
            });

            summaryViewModel.UserId = user.Id;

            summaryViewModel.ShoppingCartViewModel = this.GetShoppingCartViewModel(user);

            summaryViewModel.OrderTotal = summaryViewModel.ShoppingCartViewModel.CartTotal;

            return View(summaryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderPost(OrderSummaryViewModel orderSummaryViewModel)
        {
            var user = await this._userManager.FindByIdAsync(orderSummaryViewModel.UserId);

            var orderHeader = new OrderHeader()
            {
                AddressId = orderSummaryViewModel.AddressId,
                UserId = orderSummaryViewModel.UserId,
                Name = user.Name,
                OrderTotal = orderSummaryViewModel.OrderTotal,
                ShippinDate = orderSummaryViewModel.ShippinDate,
                OrderDate = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow,
                PaymentDueDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending.ToString()
            };
            this._unitOfWork.OrderHeaderRepository.Insert(orderHeader);
            this._unitOfWork.SaveChanges();

            var cartsToDelete = new List<ShoppingCart>();
            var orderId = orderHeader.Id;
            foreach (var cart in orderSummaryViewModel.ShoppingCartViewModel.ShoppingCarts)
            {
                var shoppingCartDetails = this._unitOfWork.ShoppingCartRepository.GetByExpression(e => e.Id == cart.Id,
                    nameof(ShoppingCart.Product));
                cartsToDelete.Add(shoppingCartDetails);
                var orderDetails = new OrderDetail()
                {
                    OrderId = orderId,
                    Count = cart.Count,
                    Price = cart.Price,
                    ProductId = shoppingCartDetails.Product.Id
                };
                this._unitOfWork.OrderDetailsRepository.Insert(orderDetails);
            }
            this._unitOfWork.SaveChanges();

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = _configuration.GetRequiredSection("DomainName").Value;
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                { "card"
                },
                    LineItems = orderSummaryViewModel.ShoppingCartViewModel.ShoppingCarts.Select(cart =>
                    {
                        var shoppingCartDetails = this._unitOfWork.ShoppingCartRepository.GetByExpression(e => e.Id == cart.Id,
                            nameof(ShoppingCart.Product));
                        var pIfno = shoppingCartDetails.Product;
                        return new SessionLineItemOptions()
                        {
                            PriceData = new SessionLineItemPriceDataOptions()
                            {
                                UnitAmountDecimal = Convert.ToDecimal(this.GetCartPrice(cart.Count, pIfno.Price, pIfno.Price50, pIfno.Price100) * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = shoppingCartDetails.Product.Title,
                                    Description = shoppingCartDetails.Product.Description,
                                }
                            },
                            Quantity = cart.Count
                        };
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = domain + $"/customer/cart/OrderConfirmation?id={orderHeader.Id}",
                    CancelUrl = domain + "/customer/cart/Index",
                };
                var service = new SessionService();
                Session session = service.Create(options);
                this._unitOfWork.OrderHeaderRepository.UpdateStripePaymentValues(orderHeader.Id, session.Id, session.PaymentIntentId);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                orderHeader.OrderStatus = OrderStatus.Approved.ToString();
                orderHeader.PaymentStatus = PaymentStatus.approvedfordelayedpayment.ToString();
                this._unitOfWork.OrderHeaderRepository.Update(orderHeader);
                this._unitOfWork.SaveChanges();
                this.DeleteShoppingCart(orderHeader);

                TempData["OrderId"] = orderHeader.Id;
                return RedirectToAction(nameof(OrderConfirmation), orderHeader.Id);
            }
        }

        /// <summary>
        /// Called from stripe, when aftre successfull order confirmation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OrderConfirmation(int? id)
        {
            var orderHeader = this._unitOfWork.OrderHeaderRepository.Find(id);
            if (orderHeader != null)
            {
                var service = new SessionService();
                var session = await service.GetAsync(orderHeader.SessionId);
                if (session?.PaymentStatus == PaymentStatus.paid.ToString())
                {
                    this.DeleteShoppingCart(orderHeader);
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, OrderStatus.Approved.ToString());
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(id, PaymentStatus.approved.ToString());
                }
                TempData["OrderId"] = orderHeader.Id;
            }
            return View();
        }

        private void DeleteShoppingCart(OrderHeader orderHeader)
        {
            var shoppingCartItems = this._unitOfWork.ShoppingCartRepository.GetAll(e => e.UserId == orderHeader.UserId);
            foreach (var shoppingCartItem in shoppingCartItems)
                this._unitOfWork.ShoppingCartRepository.Delete(shoppingCartItem);
            this._unitOfWork.SaveChanges();
        }

        private double GetCartPrice(int quantity, double price, double price50, double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity > 50 && quantity < 100)
                return price50;
            else
                return price100;
        }

        private ShoppingCartViewModel GetShoppingCartViewModel(Common.Models.Customer user)
        {
            var shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCarts = new List<ShoppingCart>()
            };
            if (user != null)
            {
                var shoppingCarts = this._unitOfWork.ShoppingCartRepository.GetAll(e => e.UserId == user.Id,
                    nameof(Common.Models.ShoppingCart.Product));
                double cartTotal = 0;
                foreach (var cart in shoppingCarts)
                {
                    var product = cart.Product;
                    cart.Price = this.GetCartPrice(cart.Count, product.Price, product.Price50, product.Price100);
                    cartTotal += cart.Price * cart.Count;
                }
                shoppingCartViewModel.CartTotal = cartTotal;
                shoppingCartViewModel.ShoppingCarts = shoppingCarts.ToList();
            }
            return shoppingCartViewModel;
        }

    }
}
