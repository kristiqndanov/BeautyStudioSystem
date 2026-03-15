using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Core.Common;

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
                throw new ArgumentException(InputValidations.InvalidDateMessage);
            }

            if (!TimeSpan.TryParse(reservationFormModel.StartTime, out TimeSpan startTime))
            {
                throw new ArgumentException(InputValidations.InvalidStartTimeMessage);
            }

            DateTime reservationStartDateTime = date.Date + startTime;

            if (reservationStartDateTime < DateTime.Now)
            {
                throw new ArgumentException(InputValidations.ReservationInPastMessage);
            }

            var service = await _servicesRepository.GetByIdAsync(reservationFormModel.ServiceId);

            if (service == null)
            {
                throw new ArgumentException(InputValidations.ServiceDoesNotExistMessage);
            }

            DateTime reservationEndDateTime = reservationStartDateTime.AddMinutes(service.Duration);

            bool isEmployeeAvailable = await _employeeRepository.IsEmployeeAvailableAsync(
                reservationFormModel.EmployeeId,
                date,
                reservationStartDateTime,
                reservationEndDateTime);

            if (!isEmployeeAvailable)
            {
                throw new InvalidOperationException(InputValidations.ReservationDuplicateMessage);
            }

            var client = await _clientsRepository.GetClientByUserId(userId);


            if (client == null)
            {
                throw new Exception(InputValidations.ClientNotFoundMessage);
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
                throw new ArgumentException(InputValidations.ReservationNotFoundMessage);
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
                throw new ArgumentException(InputValidations.ReservationNotFoundMessage);
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
                throw new ArgumentException(InputValidations.EmployeeNotFoundMessage);
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
