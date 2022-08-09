

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class OrderHeaderRepo : BulkyBookRepo<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepo(BulkyBookDbContext bulkyBookDbContext) : base(bulkyBookDbContext)
        {
        }

        public void UpdateOrderPaymentStatus(int? orderId, string? orderPaymentStatus)
        {
            var order = this.Find(orderId);
            if (order != null)
            {
                order.PaymentStatus = orderPaymentStatus;
                this.Update(order);
                this._bulkyBookDbContext.SaveChanges();
            }
        }

        public void UpdateOrderStatus(int? orderId, string? orderStatus)
        {
            var order = this.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                this.Update(order);
                this._bulkyBookDbContext.SaveChanges();
            }
        }

        public void UpdateStripePaymentValues(int? orderId, string? sessionId, string? paymentIntentId)
        {
            var order = this.Find(orderId);
            if (order != null)
            {
                order.SessionId = sessionId;
                order.PaymentIntentId = paymentIntentId;
                this.Update(order);
                this._bulkyBookDbContext.SaveChanges();
            }
        }
    }
}
