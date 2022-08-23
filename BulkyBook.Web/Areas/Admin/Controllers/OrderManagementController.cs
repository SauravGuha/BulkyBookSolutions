using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Linq.Expressions;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area(nameof(ApplicationArea.Admin))]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Common.Models.Customer> _userManager;
        private readonly IConfiguration _configuration;

        public OrderManagementController(IUnitOfWork unitOfWork, UserManager<Common.Models.Customer> userManager, IConfiguration configuration)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._configuration = configuration;
        }


        public async Task<IActionResult> Index(string orderStatus = "")
        {
            var userDetails = await this._userManager.GetUserAsync(HttpContext.User);
            var userRoles = await this._userManager.GetRolesAsync(userDetails);
            var orderViewModelList = new List<OrderViewModel>();
            var orderStatuses = Enum.GetNames(typeof(OrderStatus)).ToList();
            ViewBag.OrderStatus = orderStatuses;

            var companyId = userDetails.CompanyId;

            var orderQueryExpression = GetOrderQueryExpression(orderStatus, companyId, userRoles.FirstOrDefault(), userDetails.Id);
            var orderList = this._unitOfWork.OrderHeaderRepository.GetAll(orderQueryExpression,
                nameof(OrderHeader.OrderDetails), nameof(OrderHeader.Customer));
            foreach (var order in orderList)
            {

                orderViewModelList.Add(new OrderViewModel()
                {
                    OrderHeader = order
                });
            }

            return View(orderViewModelList);
        }

        public async Task<IActionResult> GetOrderDetails(int? id)
        {
            var orderHeader = this._unitOfWork.OrderHeaderRepository.GetByExpression(e => e.Id == id, nameof(OrderHeader.Customer), nameof(OrderHeader.OrderDetails));
            var addressDetails = this._unitOfWork.AddressRepository.Find(orderHeader.AddressId);
            var orderDetailsViewModel = new OrderDetailsViewModel()
            {
                Address = addressDetails,
                OrderHeader = orderHeader
            };
            return View("OrderDetails", orderDetailsViewModel);
        }

        [Authorize(Roles = nameof(Roles.SuperAdmin))]
        public async Task<IActionResult> DeleteOrder(int? id)
        {
            if (id.GetValueOrDefault() == 0)
            {
                TempData["Error"] = "Invalid order id.";
            }
            else
            {
                var orderHeader = this._unitOfWork.OrderHeaderRepository.Find(id);
                if (orderHeader == null)
                {
                    TempData["Error"] = $"Order id {id} not found.";
                }
                else
                {
                    //Delete the order details
                    var orderDetails = this._unitOfWork.OrderDetailsRepository.GetAll(e => e.OrderId == id);
                    foreach (var orderDetail in orderDetails)
                        this._unitOfWork.OrderDetailsRepository.Delete(orderDetail);
                    TempData["Success"] = $"Order {id} deleted successfully";
                    this._unitOfWork.SaveChanges();

                    //Delete the order header.
                    this._unitOfWork.OrderHeaderRepository.Delete(orderHeader);
                    this._unitOfWork.SaveChanges();
                }

            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(OrderDetailsViewModel orderDetailsViewModel)
        {
            var orderId = orderDetailsViewModel.OrderHeader.Id;
            var orderHeaderDetails = this._unitOfWork.OrderHeaderRepository.Find(orderId);
            if (orderHeaderDetails != null)
            {
                if (orderHeaderDetails.PaymentStatus == PaymentStatus.approved.ToString())
                {
                    var refundOptions = new RefundCreateOptions()
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeaderDetails.PaymentIntentId
                    };
                    var refundService = new RefundService();
                    var refundData = await refundService.CreateAsync(refundOptions);
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderId, OrderStatus.Cancelled.ToString());
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(orderId, PaymentStatus.refunded.ToString());
                    TempData["Success"] = $"Order {orderId} cancelled successfully.";
                }
                else
                {
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderId, OrderStatus.Cancelled.ToString());
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(orderId, PaymentStatus.refunded.ToString());
                    TempData["Success"] = $"Order {orderId} cancelled successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetails(OrderDetailsViewModel orderDetailsViewModel)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayNowOrder(OrderViewModel orderViewModel)
        {
            var orderHeader = this._unitOfWork.OrderHeaderRepository.Find(orderViewModel.OrderHeader.Id);
            var orderDetails = this._unitOfWork.OrderDetailsRepository.GetAll(e => e.OrderId == orderHeader.Id, nameof(OrderDetail.Product));

            var domain = _configuration.GetRequiredSection("DomainName").Value;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                { "card"
                },
                LineItems = orderDetails.Select(e => new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmountDecimal = Convert.ToDecimal(e.Price),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = e.Product.Title,
                            Description = e.Product.Description
                        }
                    },
                    Quantity = e.Count
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{domain}/admin/ordermanagement/OrderConfirmation?id={orderHeader.Id}",
                CancelUrl = $"{domain}/admin/ordermanagement/OrderCancel?id={orderHeader.Id}",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            this._unitOfWork.OrderHeaderRepository.UpdateStripePaymentValues(orderHeader.Id, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
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
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, OrderStatus.Approved.ToString());
                    this._unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(id, PaymentStatus.approved.ToString());
                    this._unitOfWork.OrderHeaderRepository.UpdateStripePaymentValues(orderHeader.Id, session.Id, session.PaymentIntentId);
                }
                TempData["OrderId"] = orderHeader.Id;
            }
            return View();
        }

        public IActionResult OrderCancel(int? id)
        {
            return RedirectToAction(nameof(GetOrderDetails), id);
        }


        private Expression<Func<OrderHeader, bool>> GetOrderQueryExpression(string orderStatus, int? companyId, string? userRole, string userId)
        {
            ParameterExpression pe = Expression.Parameter(typeof(OrderHeader), "oh");
            var orderStatusExpression = Expression.Property(pe, nameof(OrderHeader.OrderStatus));
            var orderStatusExpressonConstant = Expression.Constant(orderStatus);
            BinaryExpression be = Expression.NotEqual(orderStatusExpression, Expression.Constant(string.Empty));
            if (!string.IsNullOrEmpty(orderStatus))
                be = Expression.Equal(orderStatusExpression, orderStatusExpressonConstant);

            if (companyId.GetValueOrDefault() != 0)
            {
                var currentCompanyCustomerIds = this._unitOfWork.CompanyRepository
                    .GetAll(e => e.Id == companyId, nameof(Company.Customers))
                    .SelectMany(e => e.Customers.Select(e => e.Id));
                BinaryExpression companyFilterExpression = null;
                foreach (var customerId in currentCompanyCustomerIds)
                {
                    if (companyFilterExpression == null)
                        companyFilterExpression = Expression.Equal(Expression.Property(pe, nameof(OrderHeader.UserId)), Expression.Constant(customerId));

                    companyFilterExpression = Expression.Or(companyFilterExpression, Expression.Equal(Expression.Property(pe, nameof(OrderHeader.UserId)), Expression.Constant(customerId)));
                }
                be = Expression.AndAlso(be, companyFilterExpression);
            }

            if (!string.IsNullOrWhiteSpace(userRole)
                && userRole == Roles.Customer.ToString())
            {
                var leftHandSide = Expression.Constant(userId);
                var rightHandSide = Expression.Property(pe, nameof(OrderHeader.UserId));
                var customerWhereExpression = Expression.Equal(rightHandSide, leftHandSide);
                be = Expression.AndAlso(be, customerWhereExpression);
            }

            return Expression.Lambda<Func<OrderHeader, bool>>(be, new ParameterExpression[] { pe });
        }

    }
}
