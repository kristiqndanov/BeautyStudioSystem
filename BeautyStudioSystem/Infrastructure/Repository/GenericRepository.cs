
using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace BeautyStudioSystem.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public async Task AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
