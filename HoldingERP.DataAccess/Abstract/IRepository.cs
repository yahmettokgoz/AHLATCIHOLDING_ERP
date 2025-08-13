using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.DataAccess.Abstract
{
    public interface IRepository<T> where T : class
    {
        
        T GetById(int id);

       
        T? Get(Expression<Func<T, bool>> filter); 

        
        IQueryable<T> GetAll();

        
        IQueryable<T> Find(Expression<Func<T, bool>> filter);

        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

        
        void Add(T entity);

       
        void Update(T entity);

       
        void Delete(T entity);

        int SaveChanges();

    }
}
