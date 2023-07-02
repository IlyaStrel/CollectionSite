using CS.Data.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CS.Data.EFC.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        readonly ILogger _logger;
        readonly DbContext _context;
        readonly DbSet<T> _dbSet;

        public Repository(
            DbContext context, ILogger logger)
        {
            _logger = logger;
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get()
        {
            return _dbSet.AsNoTracking();
        }

        public T GetById(int id)
        {
            if (id == 0)
                return null;

            T result = _dbSet.Find(id);

            return result;
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                _dbSet.Add(entity);

            await _context.SaveChangesAsync();
        }

        public T AddAndReturn(T entity)
        {
            T result = _dbSet.Add(entity).Entity;
            _context.SaveChanges();

            return result;
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}