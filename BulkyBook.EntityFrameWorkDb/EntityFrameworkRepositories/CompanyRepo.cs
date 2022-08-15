using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    internal class CompanyRepo : BulkyBookRepo<Company>, ICompanyRepository
    {

        public CompanyRepo(BulkyBookDbContext bulkyBookDbContext):base(bulkyBookDbContext)
        {
        }
    }
}