using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeautyStudioSystem.Controllers
{
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        private readonly IServiceCategoryRepository _serviceCategoryRepository;

        public ServicesController(IServicesService servicesService, IServiceCategoryRepository serviceCategoryRepository)
        {
            _servicesService = servicesService;
            _serviceCategoryRepository = serviceCategoryRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var servicesViewModels = await _servicesService.GetAllServicesAsync();

            return View(servicesViewModels);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ServiceDetails(int id)
        {
            var serviceViewModel = await _servicesService.GetServiceAsync(id);

            return View(serviceViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditService(int id)
        {
            var serviceViewModel = await _servicesService.GetServiceAsync(id);
            var categories = await _serviceCategoryRepository.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            return View(serviceViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _servicesService.DeleteServiceAsync(id);

           return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddService()
        {
            var categories = await _serviceCategoryRepository.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddService(ServiceViewModel serviceViewModel)
        {
            var categories = await _serviceCategoryRepository.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

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
