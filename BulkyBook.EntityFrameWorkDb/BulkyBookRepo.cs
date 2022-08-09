using BulkyBook.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.EntityFrameWorkDb
{
    public class BulkyBookRepo<T> : IRepository<T> where T : class
    {
        protected BulkyBookDbContext _bulkyBookDbContext;
        private DbSet<T> _entity;

        public BulkyBookRepo(BulkyBookDbContext bulkyBookDbContext)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
            this._entity = this._bulkyBookDbContext.Set<T>();
        }

        public int Delete(T entity)
        {
            var result = _entity.Remove(entity);
            return result.Collections.Count();
        }

        public T Find(int? id)
        {
            return _entity.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> expression, params string[] includeProperties)
        {
            var tProperties = typeof(T).GetProperties();
            var entity = (IQueryable<T>)this._entity;
            foreach (var property in includeProperties)
            {
                if (tProperties.Any(e => e.Name == property))
                    entity = entity.Include(property);
            }
            return entity.Where(expression);
        }

        public IEnumerable<T> GetAll(params string[] includeProperties)
        {
            var tProperties = typeof(T).GetProperties();
            var entity = (IQueryable<T>)this._entity;
            foreach (var property in includeProperties)
            {
                if (tProperties.Any(e => e.Name == property))
                    entity = entity.Include(property);
            }
            return entity;
        }

        public T GetByExpression(Expression<Func<T, bool>> expression, params string[] includeProperties)
        {
            var tProperties = typeof(T).GetProperties();
            var entity = (IQueryable<T>)this._entity;
            foreach (var property in includeProperties)
            {
                if (tProperties.Any(e => e.Name == property))
                    entity = entity.Include(property);
            }
            return entity.FirstOrDefault(expression);
        }

        public int Insert(T entity)
        {
            var entityEntry = _entity.Add(entity);
            return entityEntry.Collections.Count();
        }

        public int Update(T entity)
        {
            var entityEntry = _entity.Update(entity);
            return entityEntry.Collections.Count();
        }
    }
}
