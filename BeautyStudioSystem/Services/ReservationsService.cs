using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Services.Contracts;
using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationsRepository _reservationsRepository;

        public ReservationsService(IReservationsRepository reservationsRepository)
        {
            _reservationsRepository = reservationsRepository;
        }


        public async Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync()
        {
            var reservationViewModels = new List<ReservationViewModel>();

            var allReservations = await _reservationsRepository.GetAllAsync();

            foreach (var reservation in allReservations)
            {
                var reservationViewModel = new ReservationViewModel
                {
                    Id = reservation.Id,
                    Date = reservation.Date.ToShortDateString(),
                    ClientName = $"{reservation.Client.FirstName} {reservation.Client.LastName}",
                    ServiceName = $"{reservation.Service.Name}",
                    StartTime = reservation.StartTime.ToShortTimeString()
                };

                reservationViewModels.Add(reservationViewModel);
            }

            return reservationViewModels;
        }

    }
}
