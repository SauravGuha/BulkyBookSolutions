

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class AddressRepo : BulkyBookRepo<Address>, IAddressRepository
    {
        public AddressRepo(BulkyBookDbContext bulkyBookDbContext) : base(bulkyBookDbContext)
        {
        }
    }
}
