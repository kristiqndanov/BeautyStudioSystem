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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddEmployeeAsync(Employee employee)
        {
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteEmployee(Employee employee)
        {
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _dbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _dbContext.Employees
                .Include(e => e.Reservations)
                .Include(e => e.ServiceCategory)
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return _dbContext.Employees.SingleOrDefault(e => e.UserId == userId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCategoryAsync(int categoryId)
        {
            return await _dbContext.Employees
                .Include(e => e.ServiceCategory)
                .Where(e => e.ServiceCategory.Any(c => c.Id == categoryId))
                .ToListAsync();
        }

        public async Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime date, DateTime startTime, DateTime endTime)
        {
            return !await _dbContext.Reservations.AnyAsync(r =>
               r.EmployeeId == employeeId &&
               r.Date == date &&
               r.StartTime < endTime &&
               r.EndTime > startTime);
        }

        public async Task UpdateEmployee(Employee employee)
        {
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();
        }
    }
}
