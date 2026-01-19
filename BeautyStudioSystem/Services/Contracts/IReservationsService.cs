using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services.Contracts
{
    public interface IReservationsService
    {
        public Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync();
    }
}
