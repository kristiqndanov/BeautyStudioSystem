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

        public Task UpdateClient(int id);

        public Task DeleteClient(int id);

    }
}
