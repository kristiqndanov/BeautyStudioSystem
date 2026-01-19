using BeautyStudioSystem.Data;
using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BeautyStudioSystem.Infrastructure.Repository
{
    public class ClientsRepository : IClientsRepository
    {
        private ApplicationDbContext _dbContext;

        public ClientsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddClientAsync(Client client)
        {
           await _dbContext.Clients.AddAsync(client);
           await _dbContext.SaveChangesAsync();
        }

        public async void DeleteClient(Client client)
        {
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _dbContext.Clients
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.Email)
            .ToListAsync();
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            return await _dbContext.Clients.SingleOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _dbContext.Clients.Include(c => c.Reservations).SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> GetClientByNameAsync(string name)
        {
            var fullNames = await _dbContext.Clients
            .Select(c => c.FirstName + " " + c.LastName)
            .ToListAsync();

            if (fullNames.Contains(name))
            {
                return await _dbContext.Clients.SingleOrDefaultAsync(c => c.FirstName + " " + c.LastName == name);
            }
          
            else
            {
                return null;
            }
            
        }

        public async void UpdateClient(Client client)
        {
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }
    }
}
