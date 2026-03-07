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
        private readonly IServicesRepository _servicesRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReservationsService(IReservationsRepository reservationsRepository, IClientsRepository clientsRepository, IServicesRepository servicesRepository, IEmployeeRepository employeeRepository)
        {
            _reservationsRepository = reservationsRepository;
            _clientsRepository = clientsRepository;
            _servicesRepository = servicesRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task AddReservationAsync(CreateReservationFormModel reservationFormModel, string userId)
        {
            if (!DateTime.TryParse(reservationFormModel.Date, out DateTime date))
            {
                throw new ArgumentException("Invalid date.");
            }

            if (!TimeSpan.TryParse(reservationFormModel.StartTime, out TimeSpan startTime))
            {
                throw new ArgumentException("Invalid start time.");
            }

            DateTime reservationStartDateTime = date.Date + startTime;

            if (reservationStartDateTime < DateTime.Now)
            {
                throw new ArgumentException("Reservation date and time cannot be in the past.");
            }

            var service = await _servicesRepository.GetByIdAsync(reservationFormModel.ServiceId);

            if (service == null)
            {
                throw new ArgumentException("Selected service does not exist.");
            }

            DateTime reservationEndDateTime = reservationStartDateTime.AddMinutes(service.Duration);

            bool isEmployeeAvailable = await _employeeRepository.IsEmployeeAvailableAsync(
                reservationFormModel.EmployeeId,
                date,
                reservationStartDateTime,
                reservationEndDateTime);

            if (!isEmployeeAvailable)
            {
                throw new InvalidOperationException("Another reservation is already booked for this employee at the same time.");
            }

            var client = await _clientsRepository.GetClientByUserId(userId);


            if (client == null)
            {
                throw new Exception("Your account doesn't exist on the database.");
            }

           

            var reservation = new Reservation
            {
                Client = client,
                ServiceId = reservationFormModel.ServiceId,
                EmployeeId = reservationFormModel.EmployeeId,
                Date = date,
                StartTime = reservationStartDateTime,
                EndTime = reservationEndDateTime
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
                    EmployeeName = $"{reservation.Employee.FirstName} {reservation.Employee.LastName}",
                    StartTime = reservation.StartTime.ToShortTimeString(),
                    EndTime = reservation.EndTime.ToShortTimeString()
                };

                reservationViewModels.Add(reservationViewModel);
            }

            return reservationViewModels;
        }

        public async Task<ReservationViewModel> GetReservationAsync(int id)
        {
            var reservation = await _reservationsRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            var reservationViewModel = new ReservationViewModel
            {
                Id = reservation.Id,
                Date = reservation.Date.ToShortDateString(),
                ClientName = $"{reservation.Client.FirstName} {reservation.Client.LastName}",
                ServiceName = reservation.Service.Name,
                EmployeeName = $"{reservation.Employee.FirstName} {reservation.Employee.LastName}",
                ClientId = reservation.ClientId,
                StartTime = reservation.StartTime.ToShortTimeString(),
                EndTime = reservation.EndTime.ToShortTimeString()
            };

            return reservationViewModel;
        }

        public async Task<IEnumerable<ReservationViewModel>> GetReservationsByEmployeeAsync(string userId)
        {
            var employee = await _employeeRepository.GetByUserIdAsync(userId);

            if (employee == null)
            {
                throw new Exception("Employee doesn't exist.");
            }

            var allReservations = await _reservationsRepository.GetAllAsync();

            return allReservations.Where(r => r.EmployeeId == employee.Id)
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    Date = r.Date.ToShortDateString(),
                    ClientName = $"{r.Client.FirstName} {r.Client.LastName}",
                    ServiceName = r.Service.Name,
                    EmployeeName = $"{r.Employee.FirstName} {r.Employee.LastName}",
                    StartTime = r.StartTime.ToShortTimeString(),
                    EndTime = r.EndTime.ToShortTimeString()
                });
        }
    }
}
