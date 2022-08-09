

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class CategoryRepo : BulkyBookRepo<Category>,ICategoryRepository
    {
        public CategoryRepo(BulkyBookDbContext bulkyBookDbContext): base(bulkyBookDbContext)
        {
        }
    }
}
