

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class CoverTypeRepo : BulkyBookRepo<CoverType>, ICoverTypeRepository
    {


        public CoverTypeRepo(BulkyBookDbContext bulkyBookDbContext): base(bulkyBookDbContext)
        {
        }

    }
}
