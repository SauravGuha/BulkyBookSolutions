

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class OrderDetailRepo : BulkyBookRepo<OrderDetail>,IOrderDetailsRepository
    {
        public OrderDetailRepo(BulkyBookDbContext bulkyBookDbContext): base(bulkyBookDbContext)
        {
        }
    }
}
