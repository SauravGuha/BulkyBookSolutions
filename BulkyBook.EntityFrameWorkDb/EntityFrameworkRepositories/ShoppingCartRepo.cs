using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class ShoppingCartRepo : BulkyBookRepo<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepo(BulkyBookDbContext bulkyBookDbContext)
            : base(bulkyBookDbContext)
        {
        }
    }
}