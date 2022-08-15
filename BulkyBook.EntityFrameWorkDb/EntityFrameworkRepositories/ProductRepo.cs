

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class ProductRepo : BulkyBookRepo<Product>, IProductRepository
    {

        public ProductRepo(BulkyBookDbContext bulkyBookDbContext) : base(bulkyBookDbContext)
        {
        }
    }
}
