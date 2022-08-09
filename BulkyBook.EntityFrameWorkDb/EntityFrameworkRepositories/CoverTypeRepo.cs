

using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class CoverTypeRepo : ICoverTypeRepository
    {
        private BulkyBookDbContext _bulkyBookDbContext;

        public CoverTypeRepo(BulkyBookDbContext bulkyBookDbContext)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
        }

        public int Delete(CoverType entity)
        {
            var entityEntry = _bulkyBookDbContext.CoverTypes.Remove(entity);
            return entityEntry.Collections.Count();
        }

        public CoverType Find(int? id)
        {
            return _bulkyBookDbContext.CoverTypes.Find(id);
        }

        public IEnumerable<CoverType> GetAll(Expression<Func<CoverType, bool>> expression, params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<CoverType>();
            var coverTypeProperties = typeof(CoverType).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes.Where(expression);
        }

        public IEnumerable<CoverType> GetAll(params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<CoverType>();
            var coverTypeProperties = typeof(CoverType).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes;
        }

        public CoverType GetByExpression(Expression<Func<CoverType, bool>> expression, params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<CoverType>();
            var coverTypeProperties = typeof(CoverType).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes.FirstOrDefault(expression);
        }

        public int Insert(CoverType entity)
        {
            var entityEntry = _bulkyBookDbContext.CoverTypes.Add(entity);
            return entityEntry.Collections.Count();
        }

        public int Update(CoverType entity)
        {
            var entityEntry = _bulkyBookDbContext.CoverTypes.Update(entity);
            return entityEntry.Collections.Count();
        }
    }
}
