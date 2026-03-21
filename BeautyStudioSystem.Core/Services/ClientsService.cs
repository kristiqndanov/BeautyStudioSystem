using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeautyStudioSystem.Core.Common;

namespace BeautyStudioSystem.Core.Services

{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepository _repo;
        private readonly UserManager<IdentityUser> _userManager;


        public ClientsService(IClientsRepository repo, UserManager<IdentityUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task AddClientAsync(ClientViewModel clientViewModel)
        {
            if (clientViewModel == null)
            {
                throw new ArgumentException(InputValidations.ClientNotFoundMessage);
            }

                var firstName = clientViewModel.FullName.Split(' ')[0];
                var lastName = clientViewModel.FullName.Split(' ').Length > 1 ? clientViewModel.FullName.Split(' ')[1] : string.Empty;
                var client = new Client
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = clientViewModel.Email,
                    Phone = clientViewModel.Phone,
                    UserId = clientViewModel.UserId
                };

               await _repo.AddClientAsync(client);
 
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _repo.GetClientByIdAsync(id);

            if (client == null)
            {
                throw new ArgumentException(InputValidations.ClientNotFoundMessage);
            }

            if (!string.IsNullOrEmpty(client.UserId))
            {
                var user = await _userManager.FindByIdAsync(client.UserId);

                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

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
                Email = c.Email,
                UserId = c.UserId
            })
            .ToList();
        }

        public async Task<ClientViewModel> GetClientByIdAsync(int id)
        {
            var client = await _repo.GetClientByIdAsync(id);

            if (client == null)
            {
                throw new ArgumentException(InputValidations.ClientNotFoundMessage);
            }

            var clientViewModel = new ClientViewModel
            {
                Id = client.Id,
                FullName = $"{client.FirstName} {client.LastName}",
                Phone = client.Phone,
                Email = client.Email,
                UserId = client.UserId
            };

            return clientViewModel;
        }

        public async Task<int> GetClientIdByUserId(string id)
        {
            var client = await _repo.GetClientByUserId(id);

            if (client == null)
            {
                throw new ArgumentException(InputValidations.ClientByUserIdNotFoundMessage, nameof(id));
            }

            return client.Id;
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

        public async Task<PaginatedResult<ClientViewModel>> GetClientsPagedAsync(string? search, int page, int pageSize)
        {
            var query = _repo.GetAllClientsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    (c.FirstName + " " + c.LastName).Contains(search) ||
                    c.Email.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var clients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var clientViewModels = clients.Select(c => new ClientViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Phone = c.Phone,
                Email = c.Email,
                UserId = c.UserId
            });

            return new PaginatedResult<ClientViewModel>
            {
                Items = clientViewModels,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task SoftDeleteClientAsync(int id)
        {
            var client = await _repo.GetClientByIdAsync(id);
            if (client != null)
            {
                await _repo.DeleteClient(id);
            }
        }

        public async Task UpdateClientAsync(ClientViewModel clientViewModel)
        {
            if (clientViewModel == null)
            {
                throw new ArgumentException(InputValidations.ClientNotFoundMessage);
            }

            var client = await _repo.GetClientByIdAsync(clientViewModel.Id);

            if (client == null)
            {
                throw new ArgumentException(InputValidations.ClientNotFoundMessage);
            }

                var names = clientViewModel.FullName.Split(' ', 2);

                client.FirstName = names[0];
                client.LastName = names.Length > 1 ? names[1] : string.Empty;
                client.Phone = clientViewModel.Phone;
                client.Email = clientViewModel.Email;

                await _repo.UpdateClient(client);
            
        }

    }
}
