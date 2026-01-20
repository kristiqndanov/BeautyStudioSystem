using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services.Contracts
{
    public interface IClientsService
    {
        public Task<IEnumerable<ClientViewModel>> GetAllClientsAsync();

        public Task<IEnumerable<ReservationViewModel>> GetClientReservations(int id);

        public Task<IEnumerable<ClientViewModel>> SearchClientsAsync(string search);

        public Task DeleteClientAsync(int id);

        public Task UpdateClientAsync(ClientViewModel clientViewModel);

        public Task<ClientViewModel> GetClientByIdAsync(int id);

        public void ValidateClient(ClientViewModel clientViewModel);
    }
}
