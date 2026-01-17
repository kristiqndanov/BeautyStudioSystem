using BeautyStudioSystem.Data;
using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BeautyStudioSystem.Infrastructure.Repository
{
    public class ReservationsRepository : IReservationsRepository
    {
        private ApplicationDbContext _dbContext;

        public ReservationsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddReservationAsync(Reservation reservation)
        {
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
        }

        public async void DeleteReservation(Reservation reservation)
        {
            _dbContext.Reservations.Remove(reservation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _dbContext.Reservations.ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllByClientNameAsync(string name)
        {
            return await _dbContext.Reservations.Where(r => r.Client.FirstName + " " + r.Client.LastName == name).ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllByServiceNameAsync(string serviceName)
        {
            return await _dbContext.Reservations.Where(r => r.Service.Name == serviceName).ToListAsync();
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
           return await _dbContext.Reservations.SingleOrDefaultAsync(r => r.Id == id);
        }

        public async void UpdateReservation(Reservation reservation)
        {
            _dbContext.Reservations.Update(reservation);
            await _dbContext.SaveChangesAsync();
        }
    }
}
