using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Infrastructure.Repository;
using BeautyStudioSystem.ViewModels;
using BeautyStudioSystem.Services.Contracts;

namespace BeautyStudioSystem.Services

{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepository _repo;

        public ClientsService(IClientsRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ClientViewModel>> GetAllClientsAsync()
        {
            var clients = await _repo.GetAllClientsAsync();

            return clients.Select(c => new ClientViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Phone = c.Phone,
                Reservations = c.Reservations
            })
            .ToList();
        }

    }
}
