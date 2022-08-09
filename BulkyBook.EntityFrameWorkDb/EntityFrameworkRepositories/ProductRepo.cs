

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class ProductRepo : IProductRepository
    {
        private BulkyBookDbContext bulkyBookDbContext;

        public ProductRepo(BulkyBookDbContext bulkyBookDbContext)
        {
            this.bulkyBookDbContext = bulkyBookDbContext;
        }

        public int Delete(Product entity)
        {
            var entityEntry = bulkyBookDbContext.Products.Remove(entity);
            return entityEntry.Collections.Count();
        }

        public Product Find(int? id)
        {
            return bulkyBookDbContext.Products.Find(id);
        }

        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>> expression, params string[] includeProperties)
        {
            IQueryable<Product> products = this.bulkyBookDbContext.Set<Product>();
            var productProperties = typeof(Product).GetProperties();
            foreach (var property in includeProperties)
            {
                if (productProperties.Any(e => e.Name == property))
                    products = products.Include(property);
            }
            return products.Where(expression);
        }

        public IEnumerable<Product> GetAll(params string[] includeProperties)
        {
            IQueryable<Product> products = this.bulkyBookDbContext.Set<Product>();
            var productProperties = typeof(Product).GetProperties();
            foreach (var property in includeProperties)
            {
                if (productProperties.Any(e => e.Name == property))
                    products = products.Include(property);
            }
            return products;
        }

        public Product GetByExpression(Expression<Func<Product, bool>> expression, params string[] includeProperties)
        {
            IQueryable<Product> products = this.bulkyBookDbContext.Set<Product>();
            var productProperties = typeof(Product).GetProperties();
            foreach (var property in includeProperties)
            {
                if (productProperties.Any(e => e.Name == property))
                    products = products.Include(property);
            }

            return products.FirstOrDefault(expression);
        }

        public int Insert(Product entity)
        {
            var entityEntry = bulkyBookDbContext.Products.Add(entity);
            return entityEntry.Collections.Count();
        }

        public int Update(Product entity)
        {
            var entityEntry = bulkyBookDbContext.Products.Update(entity);
            return entityEntry.Collections.Count();
        }
    }
}
