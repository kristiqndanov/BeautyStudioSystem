using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyStudioSystem.Data.Infrastructure.Repository
{
    public class ServiceCategoryRepository : IServiceCategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceCategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddServiceCategoryAsync(ServiceCategory serviceCategory)
        {
            await _dbContext.ServiceCategories.AddAsync(serviceCategory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceCategory(ServiceCategory serviceCategory)
        {
            _dbContext.ServiceCategories.Remove(serviceCategory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ServiceCategory>> GetAllAsync()
        {
            return await _dbContext.ServiceCategories.ToListAsync();
        }

        public async Task<ServiceCategory> GetByIdAsync(int id)
        {
            return await _dbContext.ServiceCategories.SingleOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task UpdateServiceCategory(ServiceCategory serviceCategory)
        {
           _dbContext.ServiceCategories.Update(serviceCategory);
            await _dbContext.SaveChangesAsync();
        }
    }
}
