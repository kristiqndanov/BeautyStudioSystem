using BeautyStudioSystem.ViewModels;
namespace BeautyStudioSystem.Services.Contracts
{
    public interface IServicesService
    {
        public Task<IEnumerable<ServiceViewModel>> GetAllServicesAsync();
    }
}
