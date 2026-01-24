using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services.Contracts
{
    public interface IReservationsService
    {
        public Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync();

        public Task<ReservationViewModel> GetReservationAsync(int id);
        public Task DeleteReservation(int id);
    }
}
