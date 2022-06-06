using Microsoft.EntityFrameworkCore;
using Practice.Data.Extensions;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Practice.Data
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DBContext _context;
        private readonly DbSet<T> _entities;

        public Repository(DBContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public IQueryable<T> GetAllAsNoTracking()
        {
            return _entities.AsNoTracking();
        }

        public IQueryable<T> GetAll()
        {
            return _entities;
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _entities.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<T> GetAsync(Guid id, params Expression<Func<T, object>>[] includePaths)
        {
            var result = await _entities.IncludeMultiple(includePaths).FirstOrDefaultAsync(s => s.Id == id);
            return result;
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T newEntity)
        {
            if (newEntity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Entry(newEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
