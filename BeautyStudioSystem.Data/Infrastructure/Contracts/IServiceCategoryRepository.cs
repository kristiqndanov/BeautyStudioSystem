using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Data.Infrastructure.Contracts
{
    public interface IServiceCategoryRepository
    {
        Task<ServiceCategory> GetByIdAsync(int id);

        Task<IEnumerable<ServiceCategory>> GetAllAsync();

        Task AddServiceCategoryAsync(ServiceCategory serviceCategory);

        Task UpdateServiceCategory(ServiceCategory serviceCategory);

        Task DeleteServiceCategory(ServiceCategory serviceCategory);

    }
}
