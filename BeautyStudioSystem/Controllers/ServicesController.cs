using BeautyStudioSystem.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        public async Task<IActionResult> Index()
        {
            var servicesViewModels = await _servicesService.GetAllServicesAsync();

            return View(servicesViewModels);
        }
    }
}
