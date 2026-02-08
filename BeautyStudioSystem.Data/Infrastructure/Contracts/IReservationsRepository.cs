using BeautyStudioSystem.Data.Models;

namespace BeautyStudioSystem.Data.Infrastructure.Contracts
{
    public interface IReservationsRepository
    {
        public Task<Reservation> GetByIdAsync(int id);
        public Task<IEnumerable<Reservation>> GetAllAsync();
        public Task<IEnumerable<Reservation>> GetAllByClientNameAsync(string name);
        public Task<IEnumerable<Reservation>> GetAllByServiceNameAsync(string serviceName);
        public Task<bool> ReservationExistsAsync(int serviceId, DateTime date);
        public Task AddReservationAsync(Reservation reservation);
        public Task UpdateReservation(Reservation reservation);
        public Task DeleteReservation(Reservation reservation);

    }
}
