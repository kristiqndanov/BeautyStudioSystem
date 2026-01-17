using BeautyStudioSystem.Data;
using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BeautyStudioSystem.Infrastructure.Repository
{
    public class ServicesRepository : IServicesRepository
    {
        private ApplicationDbContext _dbContext;

        public ServicesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async void DeleteService(Service service)
        {
            _dbContext.Services.Remove(service);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _dbContext.Services.ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _dbContext.Services.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async void UpdateService(Service service)
        {
            _dbContext.Services.Update(service);
            await _dbContext.SaveChangesAsync();
        }
    }
}
