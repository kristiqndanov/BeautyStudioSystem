using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Infrastructure.Contracts
{
    public interface IClientsRepository
    {
        public Task<IEnumerable<Client>> GetAllClientsAsync();
        public Task<Client> GetClientByIdAsync(int  id);

        public Task<Client> GetClientByNameAsync(string name);

        public Task<Client> GetClientByEmailAsync(string email);
        public Task AddClientAsync(Client client);

        public void UpdateClient(Client client);

        public void DeleteClient(Client client);

    }
}
