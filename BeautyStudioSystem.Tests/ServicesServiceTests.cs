using BeautyStudioSystem.Core.Services;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using Moq;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyStudioSystem.Tests
{
    public class ServicesServiceTests
    {
        private Mock<IServicesRepository> _servicesRepositoryMock;
        private ServicesService _servicesService;

        [SetUp]
        public void Setup()
        {
            _servicesRepositoryMock = new Mock<IServicesRepository>();
            _servicesService = new ServicesService(_servicesRepositoryMock.Object);
        }

        [Test]
        public async Task AddServiceAsync_ShouldThrowArgumentNullException_WhenServiceViewModelIsNull()
        {

            ServiceViewModel serviceViewModel = null;


            Assert.ThrowsAsync<ArgumentNullException>(async () => await _servicesService.AddServiceAsync(serviceViewModel));
        }

        [Test]
        public async Task AddServiceAsync_ShouldThrowException_WhenPriceIsNegative()
        {
            var serviceViewModel = new ServiceViewModel
            {
                Name = "Test Service",
                Price = -10
            };

            Assert.ThrowsAsync<Exception>(async () => await _servicesService.AddServiceAsync(serviceViewModel));
        }

        [Test]
        public async Task AddServiceAsync_WhenAllValid_ShouldAddService()
        {

            var serviceViewModel = new ServiceViewModel
            {
                Name = "Haircut",
                Price = 40.00m,
                DurationMinutes = 60,
                ServiceCategoryId = 1
            };


            await _servicesService.AddServiceAsync(serviceViewModel);

            _servicesRepositoryMock.Verify(repo => repo.AddServiceAsync(It.IsAny<Service>()), Times.Once);
        }

        [Test]
        public async Task DeleteServiceAsync_ShouldDeleteService()
        {
            int serviceId = 1;
            var service = new Service { Id = serviceId };

            _servicesRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(service);

            await _servicesService.DeleteServiceAsync(serviceId);

            _servicesRepositoryMock.Verify(repo => repo.DeleteServiceAsync(service), Times.Once);
        }

        [Test]
        public async Task GetAllServicesAsync_ShouldReturnEmptyList_WhenServicesAreEmpty()
        {
            _servicesRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Service>());

            var result = await _servicesService.GetAllServicesAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllServicesAsync_ShouldReturnValidList()
        {
            var services = new List<Service>();

            var service = new Service
            {
                Id = 1,
                Name = "Haircut",
                Price = 40.00m,
                Duration = 60,
                ServiceCategoryId = 1
            };

            services.Add(service);

            _servicesRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(services);

            var result = await _servicesService.GetAllServicesAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Haircut", result.First().Name);

        }
    }
}