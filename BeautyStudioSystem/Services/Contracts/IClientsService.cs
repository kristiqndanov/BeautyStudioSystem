using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services.Contracts
{
    public interface IClientsService
    {
        public Task<IEnumerable<ClientViewModel>> GetAllClientsAsync();
    }
}
