using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IServicesRepository _servicesRepo;

        public ServicesService(IServicesRepository servicesRepo)
        {
            _servicesRepo = servicesRepo;
        }

        public async Task AddServiceAsync(ServiceViewModel serviceViewModel)
        {
            if (serviceViewModel == null)
            {
                throw new ArgumentNullException(nameof(serviceViewModel));
            }

            if (serviceViewModel.Price <= 0)
            {
                throw new Exception("Price cannot be negative number");
            }

            var service = new Service
            {
                Name = serviceViewModel.Name,
                Price = serviceViewModel.Price,
                Reservations = serviceViewModel.Reservations
            };

            await _servicesRepo.AddServiceAsync(service);
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _servicesRepo.GetByIdAsync(id);
            await _servicesRepo.DeleteServiceAsync(service);
        }

        public async Task<IEnumerable<ServiceViewModel>> GetAllServicesAsync()
        {
            var serviceViewModels = new List<ServiceViewModel>();
            var allServices = await _servicesRepo.GetAllAsync();

            foreach (var service in allServices)
            {
                var serviceViewModel = new ServiceViewModel
                {
                    Id = service.Id,
                    Name = service.Name,
                    Price = service.Price,
                    Reservations = service.Reservations
                };

                serviceViewModels.Add(serviceViewModel);
            }

            return serviceViewModels;
        }

        public async Task<ServiceViewModel> GetServiceAsync(int id)
        {
           var service = await _servicesRepo.GetByIdAsync(id);

            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                Reservations = service.Reservations
            };

            return serviceViewModel;
        }

        public async Task UpdateServiceAsync(ServiceViewModel serviceViewModel)
        {
            if (serviceViewModel == null)
            {
                throw new ArgumentNullException(nameof(serviceViewModel));
            }

            if (serviceViewModel.Price <= 0)
            {
                throw new Exception("Price cannot be negative number");
            }

            var service = new Service
            {
                Id = serviceViewModel.Id,
                Name = serviceViewModel.Name,
                Price = serviceViewModel.Price,
                Reservations = serviceViewModel.Reservations
            };

            await _servicesRepo.UpdateServiceAsync(service);
        }
    }      
}
