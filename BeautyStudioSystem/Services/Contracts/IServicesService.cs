using BeautyStudioSystem.ViewModels;
namespace BeautyStudioSystem.Services.Contracts
{
    public interface IServicesService
    {
        public Task<IEnumerable<ServiceViewModel>> GetAllServicesAsync();

        public Task<ServiceViewModel> GetServiceAsync(int id);

        public Task UpdateServiceAsync(ServiceViewModel serviceViewModel);

        public Task DeleteServiceAsync(int id);
    }
}
