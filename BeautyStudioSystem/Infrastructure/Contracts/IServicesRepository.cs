using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Infrastructure.Contracts
{
    public interface IServicesRepository
    {
        public Task<IEnumerable<Service>> GetAllAsync();

        public Task<Service> GetByIdAsync(int id);

        public void DeleteService(Service service);

        public void UpdateService(Service service);

        public Task AddService(Service service);
    }
}
