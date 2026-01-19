using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Infrastructure.Repository;
using BeautyStudioSystem.Services.Contracts;
using BeautyStudioSystem.ViewModels;

namespace BeautyStudioSystem.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IServicesRepository _servicesRepo;

        public ServicesService(IServicesRepository servicesRepo)
        {
            _servicesRepo = servicesRepo;
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
    }      
}
