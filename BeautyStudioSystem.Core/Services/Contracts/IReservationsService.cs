using BeautyStudioSystem.Core.ViewModels;
using System.Runtime.CompilerServices;

namespace BeautyStudioSystem.Core.Services.Contracts
{
    public interface IReservationsService
    {
        public Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync();

        public Task<ReservationViewModel> GetReservationAsync(int id);
        public Task DeleteReservation(int id);

        public Task AddReservationAsync(CreateReservationFormModel reservationFormModel, string email);
    }
}
