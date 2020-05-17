using Microsoft.EntityFrameworkCore;
using SnackExchange.Web.Data;
using SnackExchange.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SnackExchange.Web.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext context;

        private DbSet<T> entities;

        string errorMessage = string.Empty;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public T GetById(Guid id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            if (id == null) throw new ArgumentNullException("entity");

            T entity = entities.SingleOrDefault(s => s.Id == id);
            entities.Remove(entity);
            context.SaveChanges();
        }

        /*public IQueryable<T> GetAllLazyLoad(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] children)
        {
            children.ToList().ForEach(x => entities.Include(x).Load());
            return entities;
        }*/

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = entities.Where(predicate);
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public T GetItem(Expression<Func<T, bool>> filter = null)
        {
            return filter == null ? context.Set<T>().FirstOrDefault() : context.Set<T>().Where(filter).FirstOrDefault();
        }

    }
}
