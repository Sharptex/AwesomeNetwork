using AwesomeNetwork.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext _db;
        private DbSet<T> _dbSet;

        public DbSet<T> Set
        {
            get;
            private set;
        }

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();

            var set = _db.Set<T>();
            set.Load();

            Set = set;
        }

        public async Task CreateAsync(T item)
        {
            await _dbSet.AddAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<T> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public void Update(T item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }
    }
}
