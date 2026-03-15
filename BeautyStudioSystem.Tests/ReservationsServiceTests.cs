using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Core.Services;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Models;


namespace BeautyStudioSystem.Tests
{
    public class ReservationsServiceTests
    {
        private Mock<IReservationsRepository> _reservationsRepositoryMock;
        private Mock<IClientsRepository> _clientsRepositoryMock;
        private Mock<IServicesRepository> _servicesRepositoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;

        private ReservationsService _reservationsService;

        [SetUp]
        public void Setup()
        {
            _reservationsRepositoryMock = new Mock<IReservationsRepository>();
            _clientsRepositoryMock = new Mock<IClientsRepository>();
            _servicesRepositoryMock = new Mock<IServicesRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();

            _reservationsService = new ReservationsService(
                _reservationsRepositoryMock.Object,
                _clientsRepositoryMock.Object,
                _servicesRepositoryMock.Object,
                _employeeRepositoryMock.Object
            );
        }

        [Test]
        public async Task AddReservationAsync_WhenDateIsInvalid_ShouldThrowException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = "test",
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));
        }

        [Test]
        public async Task AddReservationAsync_WhenStartTimeIsInvalid_ShouldThrowException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "invalid-time"
            };

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));
        }

        [Test]
        public async Task AddReservationAsync_WhenStartDateTimeIsInThePast_ShouldThrowException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));
        }

        [Test]
        public async Task AddReservationAsync_WhenServiceIsNull_ShouldThrowArgumentException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            _servicesRepositoryMock.Setup(repo => repo.GetByIdAsync(formModel.ServiceId))
                .ReturnsAsync((Service)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));

            _servicesRepositoryMock.Verify(repo => repo.GetByIdAsync(formModel.ServiceId), Times.Once);
        }

        [Test]
        public async Task AddReservationAsync_WhenEmployeeIsNotAvailable_ShouldThrowInvalidOperationException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            var service = new Service { Id = formModel.ServiceId, Duration = 60 };

            _servicesRepositoryMock.Setup(repo => repo.GetByIdAsync(formModel.ServiceId))
                .ReturnsAsync(service);

            _employeeRepositoryMock.Setup(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));

            _servicesRepositoryMock.Verify(repo => repo.GetByIdAsync(formModel.ServiceId), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public async Task AddReservationAsync_WhenClientIsNull_ShouldThrowException()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            var service = new Service { Id = formModel.ServiceId, Duration = 60 };

            _servicesRepositoryMock.Setup(repo => repo.GetByIdAsync(formModel.ServiceId))
                .ReturnsAsync(service);

            _employeeRepositoryMock.Setup(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _clientsRepositoryMock.Setup(repo => repo.GetClientByUserId("user-id"))
                .ReturnsAsync((Client)null);

            Assert.ThrowsAsync<Exception>(async () => await _reservationsService.AddReservationAsync(formModel, "user-id"));

            _servicesRepositoryMock.Verify(repo => repo.GetByIdAsync(formModel.ServiceId), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()), Times.Once);
            _clientsRepositoryMock.Verify(repo => repo.GetClientByUserId("user-id"), Times.Once);
        }

        [Test]
        public async Task AddReservationAsync_WhenAllValid_ShouldAddReservation()
        {
            var formModel = new CreateReservationFormModel
            {
                Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                ServiceId = 1,
                EmployeeId = 1,
                StartTime = "10:00"
            };

            var service = new Service { Id = formModel.ServiceId, Duration = 60 };
            var client = new Client { Id = 1, UserId = "user-id" };

            _servicesRepositoryMock.Setup(repo => repo.GetByIdAsync(formModel.ServiceId))
                .ReturnsAsync(service);

            _employeeRepositoryMock.Setup(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _clientsRepositoryMock.Setup(repo => repo.GetClientByUserId("user-id"))
                .ReturnsAsync(client);

            await _reservationsService.AddReservationAsync(formModel, "user-id");

            _servicesRepositoryMock.Verify(repo => repo.GetByIdAsync(formModel.ServiceId), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.IsEmployeeAvailableAsync(
                formModel.EmployeeId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()), Times.Once);
            _clientsRepositoryMock.Verify(repo => repo.GetClientByUserId("user-id"), Times.Once);
            _reservationsRepositoryMock.Verify(repo => repo.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);

        }

        [Test]
        public async Task DeleteReservation_WhenReservationDoesNotExist_ShouldThrowArgumentException()
        {
            _reservationsRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Reservation)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.DeleteReservation(1));

            _reservationsRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteReservation_WhenReservationExists_ShouldDeleteReservation()
        {
            var reservation = new Reservation { Id = 1 };

            _reservationsRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(reservation);

            await _reservationsService.DeleteReservation(1);
        }

        [Test]
        public async Task GetAllReservationsAsync_ShouldReturnEmptyListWhenNoReservations()
        {
            _reservationsRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Reservation>());

            var result = await _reservationsService.GetAllReservationsAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllReservationsAsync_ShouldReturnValid_WhenAListIsPopulated()
        {
            var reservations = new List<Reservation>();
            var reservation = new Reservation
            {
                Id = 1,
                Client = new Client { FirstName = "Anna", LastName = "Vasileva" },
                Service = new Service { Name = "Haircut" },
                Employee = new Employee { FirstName = "Maria", LastName = "Todorova" },
                Date = DateTime.Now.AddDays(1),
                StartTime = DateTime.Now.AddDays(1).AddHours(10),
                EndTime = DateTime.Now.AddDays(1).AddHours(11)

            };

            reservations.Add(reservation);

            _reservationsRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(reservations);

            var result = await _reservationsService.GetAllReservationsAsync();
            var resultList = result.ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Maria Todorova", resultList[0].EmployeeName);
            Assert.AreEqual("Anna Vasileva", resultList[0].ClientName);
            Assert.AreEqual("Haircut", resultList[0].ServiceName);
        }

        [Test]
        public async Task GetReservationAsync_ShouldThrowArgumentException_WhenReservationIsNull()
        {
            _reservationsRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Reservation)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.GetReservationAsync(1));
        }

        [Test]

        public async Task GetReservationAsync_ShouldReturnValid_WhenReservationExists()
        {
            var reservation = new Reservation
            {
                Id = 1,
                Client = new Client { FirstName = "Anna", LastName = "Vasileva" },
                Service = new Service { Name = "Haircut" },
                Employee = new Employee { FirstName = "Maria", LastName = "Todorova" },
                Date = DateTime.Now.AddDays(1),
                StartTime = DateTime.Now.AddDays(1).AddHours(10),
                EndTime = DateTime.Now.AddDays(1).AddHours(11)

            };

            _reservationsRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(reservation);

            var result = await _reservationsService.GetReservationAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Maria Todorova", result.EmployeeName);
            Assert.AreEqual("Anna Vasileva", result.ClientName);
            Assert.AreEqual("Haircut", result.ServiceName);
        }

        [Test]
        public async Task GetReservationsByEmployeeAsync_ShouldThrowException_WhenEmployeeIsNull()
        {
            _employeeRepositoryMock.Setup(repo => repo.GetByUserIdAsync("user-id"))
                .ReturnsAsync((Employee)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _reservationsService.GetReservationsByEmployeeAsync("user-id"));

            _employeeRepositoryMock.Verify(repo => repo.GetByUserIdAsync("user-id"), Times.Once);
        }

        [Test]
        public async Task GetReservationsByEmployeeAsync_ShouldReturnValid()
        {
         
            var employee = new Employee { Id = 1, UserId = "user-id", FirstName = "Maria", LastName = "Todorova" };
            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = 1,
                    EmployeeId = 1,
                    Client = new Client { FirstName = "Anna", LastName = "Vasileva" },
                    Service = new Service { Name = "Haircut" },
                    Employee = employee,
                    Date = DateTime.Now.AddDays(1),
                    StartTime = DateTime.Now.AddDays(1).AddHours(10),
                    EndTime = DateTime.Now.AddDays(1).AddHours(11)
                }
            };

           _employeeRepositoryMock.Setup(repo => repo.GetByUserIdAsync("user-id")).ReturnsAsync(employee);

            _reservationsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(reservations);

            var result = await _reservationsService.GetReservationsByEmployeeAsync("user-id");
            var resultList = result.ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Maria Todorova", resultList[0].EmployeeName);
            Assert.AreEqual("Anna Vasileva", resultList[0].ClientName);
            Assert.AreEqual("Haircut", resultList[0].ServiceName);
        }
    }
}