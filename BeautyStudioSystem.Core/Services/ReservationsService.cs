using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Core.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationsRepository _reservationsRepository;
        private readonly IClientsRepository _clientsRepository;

        public ReservationsService(IReservationsRepository reservationsRepository, IClientsRepository clientsRepository)
        {
            _reservationsRepository = reservationsRepository;
            _clientsRepository = clientsRepository;
        }

        public async Task AddReservationAsync(CreateReservationFormModel reservationViewModel)
        {
            DateTime.TryParse(reservationViewModel.Date, out DateTime date);
            TimeSpan.TryParse(reservationViewModel.StartTime, out TimeSpan startTime);

            if (date == null)
            {
                throw new Exception("Invalid date.");
            }

            if (startTime == null)
            {
                throw new Exception("Invalid start time.");
            }

            DateTime reservationDateTime = date.Date + startTime;

            if (reservationDateTime < DateTime.Now)
            {
                throw new Exception("Reservation date and time cannot be in the past.");
            }

            bool alreadyExists = await _reservationsRepository.ReservationExistsAsync(reservationViewModel.ServiceId, date);

            if (alreadyExists)
            {
                throw new Exception("A reservation for the selected service on the specified date already exists.");
            }

            var client = await _clientsRepository.GetClientByEmailAsync(reservationViewModel.Email);

            if (client == null)
            {
                throw new Exception("Client with the provided email does not exist.");
            }

           

            var reservation = new Reservation
            {
                Client = client,
                ServiceId = reservationViewModel.ServiceId,
                Date = date,
                StartTime = reservationDateTime
            };

            await _reservationsRepository.AddReservationAsync(reservation);
        }

        public async Task DeleteReservation(int id)
        {
            var reservation = await _reservationsRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            await _reservationsRepository.DeleteReservation(reservation);
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

        public async Task<ReservationViewModel> GetReservationAsync(int id)
        {
            var reservation = await _reservationsRepository.GetByIdAsync(id);

            var reservationViewModel = new ReservationViewModel
            {
                Id = reservation.Id,
                Date = reservation.Date.ToShortDateString(),
                ClientName = $"{reservation.Client.FirstName} {reservation.Client.LastName}",
                ServiceName = $"{reservation.Service.Name}",
                ClientId = reservation.ClientId,
                StartTime = reservation.StartTime.ToShortTimeString()
            };

            return reservationViewModel;
        }
    }
}
