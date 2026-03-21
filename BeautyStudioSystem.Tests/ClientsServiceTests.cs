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
                Phone = "1234567890",
                UserId = "user123",
            };

            await _clientsRepositoryMock.Object.AddClientAsync(client);

            _clientsRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Data.Models.Client>()), Times.Once);
        }

        [Test]
        public async Task DeleteClientAsync_ShouldThrowArgumentException_IfClientNotFound()
        {
            int clientId = 1;
            _clientsRepositoryMock.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync((Client)null);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _clientsService.DeleteClientAsync(clientId));
        }

        [Test]
        public async Task DeleteClientAsync_ShouldDeleteUser_IfUserIdIsNotNull()
        {
            int clientId = 1;
            var client = new Client
            {
                Id = clientId,
                UserId = "user123"
            };

            _clientsRepositoryMock.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);
            _userManagerMock.Setup(u => u.FindByIdAsync(client.UserId)).ReturnsAsync(new IdentityUser { Id = client.UserId });

            await _clientsService.DeleteClientAsync(clientId);

            _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<IdentityUser>()), Times.Once);
        }

        [Test]
        public async Task DeleteClientAsync_ShouldDeleteClient_IfClientExists()
        {
            int clientId = 1;
            var client = new Client
            {
                Id = clientId,
                UserId = "user123"
            };

            _clientsRepositoryMock.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);

            await _clientsService.DeleteClientAsync(clientId);

            _clientsRepositoryMock.Verify(r => r.DeleteClient(clientId), Times.Once);
        }

        [Test]
        public async Task GetAllClientsAsync_ShouldReturnEmptyList_IfThereAreNoClients()
        {
            _clientsRepositoryMock.Setup(r => r.GetAllClientsAsync()).ReturnsAsync(new List<Client>());

            var result = await _clientsService.GetAllClientsAsync();

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllClientsAsync_ShouldReturnCorrectList_IfThereAreClients()
        {
            var clients = new List<Client>();
            var client1 = new Client
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@test.com",
                Phone = "1234567890",
                UserId = "user123"
            };

            var client2 = new Client
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "janesmith@test.com",
                Phone = "0987654321",
                UserId = "user456"
            };

            clients.Add(client1);
            clients.Add(client2);

            _clientsRepositoryMock.Setup(r => r.GetAllClientsAsync()).ReturnsAsync(clients);

            var result = await _clientsService.GetAllClientsAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("John Doe", result.First().FullName);
            Assert.AreEqual("Jane Smith", result.Last().FullName);
        }

        [Test]
        public async Task GetClientByIdAsync_ShouldThrow_IfClientIsNull()
        {
            int clientId = 1;
            _clientsRepositoryMock.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync((Client)null);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _clientsService.GetClientByIdAsync(clientId));
        }

        [Test]
        public async Task GetClientByIdAsync_ShouldPass_IfClientIsFound()
        {
            int clientId = 1;
            var client = new Client
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@test.com",
                Phone = "1234567890",
                UserId = "user123"
            };

            _clientsRepositoryMock.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);

            var clientViewModel = new ClientViewModel
            {
                Id = clientId,
                FullName = $"{client.FirstName} {client.LastName}",
                Email = client.Email,
                Phone = client.Phone,
                UserId = client.UserId
            };

            var result = await _clientsService.GetClientByIdAsync(clientId);

            Assert.IsNotNull(result);
            Assert.AreEqual(clientViewModel.UserId, result.UserId);
            Assert.AreEqual(clientViewModel.FullName, result.FullName);
        }
    }
}
