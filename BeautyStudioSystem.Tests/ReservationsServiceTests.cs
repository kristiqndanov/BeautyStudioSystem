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
    }
}