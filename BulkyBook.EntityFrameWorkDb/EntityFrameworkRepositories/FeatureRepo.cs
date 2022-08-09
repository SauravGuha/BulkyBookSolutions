
using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class FeatureRepo : IFeatureRepository
    {
        private BulkyBookDbContext _bulkyBookDbContext;

        public FeatureRepo(BulkyBookDbContext bulkyBookDbContext)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
        }

        public int Delete(Feature entity)
        {
            var entityEntry = _bulkyBookDbContext.Features.Remove(entity);
            return entityEntry.Collections.Count();
        }

        public Feature Find(int? id)
        {
            return _bulkyBookDbContext.Features.Find(id);
        }

        public IEnumerable<Feature> GetAll(Expression<Func<Feature, bool>> expression, params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<Feature>();
            var coverTypeProperties = typeof(Feature).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes.Where(expression);
        }

        public IEnumerable<Feature> GetAll(params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<Feature>();
            var coverTypeProperties = typeof(Feature).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes;
        }

        public Feature GetByExpression(Expression<Func<Feature, bool>> expression, params string[] includeProperties)
        {
            var coverTypes = this._bulkyBookDbContext.Set<Feature>();
            var coverTypeProperties = typeof(Feature).GetProperties();
            foreach (var property in includeProperties)
            {
                if (coverTypeProperties.Any(e => e.Name == property))
                    coverTypes.Include(property);
            }
            return coverTypes.FirstOrDefault(expression);
        }

        public int Insert(Feature entity)
        {
            var entityEntry = _bulkyBookDbContext.Features.Add(entity);
            return entityEntry.Collections.Count();
        }

        public int Update(Feature entity)
        {
            var entityEntry = _bulkyBookDbContext.Features.Update(entity);
            return entityEntry.Collections.Count();
        }
    }
}
