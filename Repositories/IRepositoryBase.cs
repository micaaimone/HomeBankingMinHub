using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HomeBankingMinHub.Repositories
{
    //T es un generico
    public interface IRepositoryBase <T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}
