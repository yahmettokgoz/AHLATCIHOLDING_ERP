using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Business.Abstract
{
    public interface IGenericService<T> where T : class
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetById(int id);
        IQueryable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
        T? Get(Expression<Func<T, bool>> filter);

        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        void SaveChanges();
    }
}
