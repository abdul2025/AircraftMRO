using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Data;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        // which dynamically resolves:
        // and is the dynamic generic access mechanism.
        protected readonly DbSet<T> _dbSet;
        protected readonly ApplicationDbContext _context;


        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}