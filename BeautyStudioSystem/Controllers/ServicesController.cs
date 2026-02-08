using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
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

        public async Task<IActionResult> ServiceDetails(int id)
        {
            var serviceViewModel = await _servicesService.GetServiceAsync(id);

            return View(serviceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var serviceViewModel = await _servicesService.GetServiceAsync(id);

            return View(serviceViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> EditService(ServiceViewModel serviceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceViewModel);
            }

            try
            {
                await _servicesService.UpdateServiceAsync(serviceViewModel);
                return RedirectToAction("Index");
            }

            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> DeleteService(int id)
        {
            await _servicesService.DeleteServiceAsync(id);

           return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddService()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> AddService(ServiceViewModel serviceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceViewModel);
            }

            try
            {
                await _servicesService.AddServiceAsync(serviceViewModel);
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }
    }
}
