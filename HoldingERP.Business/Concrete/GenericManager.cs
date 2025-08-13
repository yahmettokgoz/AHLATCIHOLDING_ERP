using HoldingERP.Business.Abstract;
using HoldingERP.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Business.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : class
    {
        protected readonly IRepository<T> _repository;

        public GenericManager(IRepository<T> repository)
        {
            _repository = repository;
        }

        public void Create(T entity)
        {
            _repository.Add(entity);

        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _repository.Find(filter);
        }

        public IQueryable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
        }

        public T? Get(Expression<Func<T, bool>> filter)
        {
            return _repository.Get(filter);
        }

        public void SaveChanges()
        {
            _repository.SaveChanges();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            return _repository.GetAllIncluding(includeProperties);
        }
    }
}
