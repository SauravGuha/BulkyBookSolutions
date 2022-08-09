

using BulkyBook.Common.Models;

namespace BulkyBook.Common.Interfaces.ModelRepositories
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public void UpdateOrderStatus(int? orderId, string? orderStatus);

        public void UpdateStripePaymentValues(int? orderId, string? sessionId, string? paymentIntentId);

        public void UpdateOrderPaymentStatus(int? orderId, string? orderPaymentStatus);
    }
}
