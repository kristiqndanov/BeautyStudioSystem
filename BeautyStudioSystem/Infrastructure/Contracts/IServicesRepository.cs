using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Infrastructure.Contracts
{
    public interface IServicesRepository
    {
        public Task<IEnumerable<Service>> GetAllAsync();

        public Task<Service> GetByIdAsync(int id);

        public Task DeleteServiceAsync(Service service);

        public Task UpdateServiceAsync(Service service);

        public Task AddServiceAsync(Service service);
    }
}
