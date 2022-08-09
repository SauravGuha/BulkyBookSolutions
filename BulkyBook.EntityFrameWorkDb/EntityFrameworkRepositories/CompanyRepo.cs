using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    internal class CompanyRepo : ICompanyRepository
    {
        private BulkyBookDbContext _bulkyBookDbContext;

        public CompanyRepo(BulkyBookDbContext bulkyBookDbContext)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
        }

        public int Delete(Company entity)
        {
            var entityEntry = _bulkyBookDbContext.Companies.Remove(entity);
            return entityEntry.Collections.Count();
        }

        public Company Find(int? id)
        {
            return _bulkyBookDbContext.Companies.Find(id);
        }

        public IEnumerable<Company> GetAll(Expression<Func<Company, bool>> expression, params string[] includeProperties)
        {
            var categorySet = this._bulkyBookDbContext.Set<Company>();
            var categoryProperties = typeof(Company).GetProperties();
            foreach (var property in includeProperties)
            {
                if (categoryProperties.Any(e => e.Name == property))
                    categorySet.Include(property);
            }
            return categorySet.Where(expression);
        }

        public IEnumerable<Company> GetAll(params string[] includeProperties)
        {
            var categorySet = this._bulkyBookDbContext.Set<Company>();
            var categoryProperties = typeof(Company).GetProperties();
            foreach (var property in includeProperties)
            {
                if (categoryProperties.Any(e => e.Name == property))
                    categorySet.Include(property);
            }
            return categorySet;
        }

        public Company GetByExpression(Expression<Func<Company, bool>> expression, params string[] includeProperties)
        {
            var categorySet = this._bulkyBookDbContext.Set<Company>();
            var categoryProperties = typeof(Company).GetProperties();
            foreach (var property in includeProperties)
            {
                if (categoryProperties.Any(e => e.Name == property))
                    categorySet.Include(property);
            }
            return categorySet.FirstOrDefault(expression);
        }

        public int Insert(Company entity)
        {
            var entityEntry = _bulkyBookDbContext.Companies.Add(entity);
            return entityEntry.Collections.Count();
        }

        public int Update(Company entity)
        {
            var entityEntry = _bulkyBookDbContext.Companies.Update(entity);
            return entityEntry.Collections.Count();
        }
    }
}