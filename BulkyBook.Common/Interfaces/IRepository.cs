
using System.Linq.Expressions;

namespace BulkyBook.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Finds and returns the element. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>May return null</returns>
        T Find(int? id);

        T GetByExpression(Expression<Func<T, bool>> expression, params string[] includeProperties);

        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression, params string[] includeProperties);

        IEnumerable<T> GetAll(params string[] includeProperties);

        Task<List<T>> GetAllAsync(params string[] includeProperties);

        int Delete(T entity);

        int Update(T entity);

        int Insert(T entity);
    }
}
