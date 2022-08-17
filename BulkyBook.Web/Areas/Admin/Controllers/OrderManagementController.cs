using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area(nameof(ApplicationArea.Admin))]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Common.Models.Customer> _userManager;

        public OrderManagementController(IUnitOfWork unitOfWork, UserManager<Common.Models.Customer> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        [Authorize(Roles = nameof(Roles.Admin) 
            + "," + nameof(Roles.SuperAdmin) 
            + "," + nameof(Roles.CompanyUser))]
        public async Task<IActionResult> Index(string orderStatus = "")
        {
            var orderViewModelList = new List<OrderViewModel>();
            var orderStatuses = Enum.GetNames(typeof(OrderStatus)).ToList();
            ViewBag.OrderStatus = orderStatuses;

            var userDetails = await this._userManager.GetUserAsync(HttpContext.User);
            var companyId = userDetails.CompanyId;

            var orderQueryExpression = GetOrderQueryExpression(orderStatus, companyId);
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

        private Expression<Func<OrderHeader, bool>> GetOrderQueryExpression(string orderStatus, int? companyId)
        {
            ParameterExpression pe = Expression.Parameter(typeof(OrderHeader), "oh");
            var orderStatusExpression = Expression.Property(pe, nameof(OrderHeader.OrderStatus));
            var orderStatusExpressonConstant = Expression.Constant(orderStatus);
            BinaryExpression be = Expression.NotEqual(orderStatusExpression, Expression.Constant(string.Empty));
            if (!string.IsNullOrEmpty(orderStatus))
                be = Expression.Equal(orderStatusExpression, orderStatusExpressonConstant);

            if (companyId.GetValueOrDefault() != 0)
            {
                var currentCompanyCustomerIds = this._unitOfWork.CompanyRepository.GetAll(e => e.Id == companyId, nameof(Company.Customers))
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

            return Expression.Lambda<Func<OrderHeader, bool>>(be, new ParameterExpression[] { pe });
        }

    }
}
