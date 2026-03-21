using BeautyStudioSystem.Core.Services;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyStudioSystem.Tests
{
    public class ClientsServiceTests
    {
        private Mock<IClientsRepository> _clientsRepositoryMock;
        private Mock<UserManager<IdentityUser>> _userManagerMock;

        private IClientsService _clientsService;

        [SetUp]
        public void Setup()
        {
            _clientsRepositoryMock = new Mock<IClientsRepository>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null);

            _clientsService = new ClientsService(_clientsRepositoryMock.Object, _userManagerMock.Object);
        }

        [Test]
        public async Task AddClientAsync_ShouldThrowArgumentException_IfClientViewModelIsNull()
        {
            ClientViewModel clientViewModel = null;
            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _clientsService.AddClientAsync(clientViewModel));
        }

        [Test]
        public async Task AddClientAsync_ShouldPass_IfLastNameIsEmpty()
        {
            ClientViewModel clientViewModel = new ClientViewModel()
            {
                FullName = "John",
                Email = "test@test.com",
                Phone = "1234567890"
            };

            await _clientsService.AddClientAsync(clientViewModel);

            _clientsRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Data.Models.Client>()), Times.Once);
        }

        [Test]
        public async Task AddClientAsync_ShouldPass_IfAllIsValid()
        {
            Client client = new Client()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@test.com",
                Phone= "1234567890",
                UserId = "user123",
            };

            await _clientsRepositoryMock.Object.AddClientAsync(client);

            _clientsRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Data.Models.Client>()), Times.Once);
        }

        

    }
}
