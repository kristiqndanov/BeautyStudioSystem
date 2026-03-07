using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using BeautyStudioSystem.Data.Infrastructure.Contracts;

namespace BeautyStudioSystem.Controllers
{
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;
        private readonly IServicesService _servicesService;
        private readonly IEmployeeRepository _employeeRepository;

        public ReservationsController(IReservationsService reservationsService, IServicesService servicesService, IEmployeeRepository employeeRepository)
        {
            _reservationsService = reservationsService;
            _servicesService = servicesService;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var reservationsViewModels = _reservationsService.GetAllReservationsAsync();

            return View(reservationsViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReservation(int id)
        {

            var reservation = await _reservationsService.GetReservationAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }


            await _reservationsService.DeleteReservation(id);

            TempData["Message"] = "Reservation deleted successfully.";

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(
                    "ClientReservations",
                    "Clients",
                    new { id = reservation.ClientId }
                );
            }

            return RedirectToAction("MyReservations", "Clients");
        }

        [HttpGet]

        public async Task<IActionResult> CreateReservation()
        {
            var services = await _servicesService.GetAllServicesAsync();

            ViewBag.Services = services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            ViewBag.ServiceCategories = services.ToDictionary(s => s.Id.ToString(), s => s.ServiceCategoryId.ToString());

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> CreateReservation(CreateReservationFormModel reservationViewModel)
        {
            var services = await _servicesService.GetAllServicesAsync();

            ViewBag.Services = services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            ViewBag.ServiceCategories = services.ToDictionary(s => s.Id.ToString(), s => s.ServiceCategoryId.ToString());

            if (!ModelState.IsValid)
            {

                return View(reservationViewModel);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _reservationsService.AddReservationAsync(reservationViewModel, userId);

                TempData["Message"] = "Reservation created successfully.";

                return RedirectToAction("Index", "Home");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesByServiceCategory(int categoryId)
        {
            var employees = await _employeeRepository.GetEmployeesByCategoryAsync(categoryId);

            var jsonEmployees = employees.Select(e => new
            {
                Id = e.Id,
                name = $"{e.FirstName} {e.LastName}"
            });

            return Json(jsonEmployees);

        }
    }
}
