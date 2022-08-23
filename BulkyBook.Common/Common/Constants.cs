namespace BulkyBook.Common
{
    public class Constants
    {
        public enum Roles
        {
            #region App internal user roles

            SuperAdmin,
            Admin,

            #endregion

            #region App external user roles

            Customer,
            CompanyUser

            #endregion
        }

        public enum OrderStatus
        {
            Pending,
            Approved,
            Processing,
            Dispatched,
            Delivered,
            Returned,
            Cancelled
        }

        public enum PaymentStatus
        {
            pending,
            approved,
            paid,
            approvedfordelayedpayment,
            refunded
        }

        public enum ApplicationArea
        {
            Admin,
            Customer,
            Identity
        }
    }
}
