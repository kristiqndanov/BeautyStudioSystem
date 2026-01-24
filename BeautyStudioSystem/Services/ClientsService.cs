using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Infrastructure.Repository;
using BeautyStudioSystem.ViewModels;
using BeautyStudioSystem.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Services

{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepository _repo;

        public ClientsService(IClientsRepository repo)
        {
            _repo = repo;
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _repo.GetClientByIdAsync(id);

            if (client != null)
            {
                await _repo.DeleteClient(id);
            }
        }

        public async Task<IEnumerable<ClientViewModel>> GetAllClientsAsync()
        {
            var clients = await _repo.GetAllClientsAsync();

            return clients.Select(c => new ClientViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Phone = c.Phone,
                Email = c.Email
            })
            .ToList();
        }

        public async Task<ClientViewModel> GetClientByIdAsync(int id)
        {
            var client = await _repo.GetClientByIdAsync(id);

            if (client == null)
            {
                return null;
            }

            var clientViewModel = new ClientViewModel
            {
                Id = client.Id,
                FullName = $"{client.FirstName} {client.LastName}",
                Phone = client.Phone,
                Email = client.Email
            };

            return clientViewModel;
        }

        public async Task<IEnumerable<ReservationViewModel>> GetClientReservations(int id)
        {
            var client = await _repo.GetClientByIdAsync(id); 

            var reservationViewModels = new List<ReservationViewModel>();

            if (client == null || !client.Reservations.Any())
            {
                return reservationViewModels;
            }

            foreach (var reservation in client.Reservations)
            {
                var reservationViewModel = new ReservationViewModel
                {
                    Id = reservation.Id,
                    Date = reservation.Date.ToShortDateString(),
                    ClientName = $"{client.FirstName} {client.LastName}",
                    ServiceName = $"{reservation.Service.Name}",
                    StartTime = reservation.StartTime.ToShortTimeString()
                };

                reservationViewModels.Add(reservationViewModel);
            }
             return reservationViewModels;
        }

        public async Task<IEnumerable<ClientViewModel>> SearchClientsAsync(string search)
        {
            var clients = await _repo.GetAllClientsAsync();
            var regex = new Regex("@[^@\\s]+\\.[^@\\s]+");

            var clientsViewModels = clients.Select(c => new ClientViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Email = c.Email,
                Phone = c.Phone
            }).ToList();

            if (string.IsNullOrEmpty(search))
            {
                return clientsViewModels;
            }

            else 
            {
                if (regex.IsMatch(search))
                {
                    clientsViewModels = clientsViewModels
                    .Where(c => c.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                    return clientsViewModels;
                }

                else
                {
                    clientsViewModels = clientsViewModels
                    .Where(c => c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                    return clientsViewModels;

                }

            }
        }

        public async Task UpdateClientAsync(ClientViewModel clientViewModel)
        {
            var client = await _repo.GetClientByIdAsync(clientViewModel.Id);

            if (client != null)
            {
                var names = clientViewModel.FullName.Split(' ', 2);

                client.FirstName = names[0];
                client.LastName = names.Length > 1 ? names[1] : string.Empty;
                client.Phone = clientViewModel.Phone;
                client.Email = clientViewModel.Email;

                await _repo.UpdateClient(client);
            }
        }

    }
}
