using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Data.Infrastructure.Contracts
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);

        Task<IEnumerable<Employee>> GetAllAsync();
        Task<bool> IsEmployeeAvailableAsync(int employeeId, DateTime date, DateTime startTime, DateTime endTime);
        Task AddEmployeeAsync(Employee employee);

        Task UpdateEmployee(Employee employee);
        Task DeleteEmployee(Employee employee);
    }
}
