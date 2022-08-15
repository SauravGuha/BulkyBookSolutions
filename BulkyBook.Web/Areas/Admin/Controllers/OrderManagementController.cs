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
    [Authorize(Roles = "SuperAdmin,Admin,CompanyUser")]

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

        public async Task<IActionResult> OrderDetails(int? id)
        {
            var orderHeader = this._unitOfWork.OrderHeaderRepository.GetByExpression(e => e.Id == id, nameof(OrderHeader.OrderDetails));

            return View();
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
