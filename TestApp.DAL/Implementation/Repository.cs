using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TestApp.DAL.Extensions;
using TestApp.DAL.Interfaces;

namespace TestApp.DAL.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _db;

        public Repository(DbContext db)
        {
            _db = db;
        }

        public IQueryable<T> GetAll() => _db.Set<T>().AsQueryable();

        public Task<T> GetByPredicateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return _db.Set<T>().IncludeMultiple(includes).FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(int id) => await _db.Set<T>().FindAsync(id);

        public async Task<T> GetByIdAsync(Guid id) => await _db.Set<T>().FindAsync(id);
        
        public void Create(T item) => _db.Set<T>().Add(item);

        public void Remove(T item) => _db.Set<T>().Remove(item);

        public void RemoveRange(IEnumerable<T> entities) => _db.Set<T>().RemoveRange(entities);

        public void Update(T item) => _db.Entry(item).State = EntityState.Modified;
    }
}
