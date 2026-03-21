using BeautyStudioSystem.Data;
using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BeautyStudioSystem.Data.Infrastructure.Repository
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

        public async Task DeleteClient(int id)
        {
            var client = await _dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return;
            }   

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


        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _dbContext.Clients
                .Include(c => c.Reservations)
                .ThenInclude(r => r.Service)  
                .SingleOrDefaultAsync(c => c.Id == id);
        }


        public async Task<Client> GetClientByUserId(string userId)
        {
            return await _dbContext.Clients.SingleOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateClient(Client client)
        {
            if (client != null)
            {
                _dbContext.Clients.Update(client);
                await _dbContext.SaveChangesAsync();
            }
            
        }

        public IQueryable<Client> GetAllClientsQueryable()
        {
            return _dbContext.Clients
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .AsQueryable();
        }
    }
}
