using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Common;
using BeautyStudioSystem.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace BeautyStudioSystem.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;
        private readonly IServicesService _servicesService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IClientsService _clientsService;

        public ReservationsController(IReservationsService reservationsService, IServicesService servicesService, IEmployeeRepository employeeRepository, UserManager<IdentityUser> userManager, IClientsService clientsService)
        {
            _reservationsService = reservationsService;
            _servicesService = servicesService;
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            _clientsService = clientsService;
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

            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                var clientId = await _clientsService.GetClientIdByUserId(user.Id);
                if (reservation.ClientId != clientId)
                {
                    return Forbid();
                }
            }

            await _reservationsService.DeleteReservation(id);

            TempData["Message"] = ValidationAndErrorMessageConstants.ReservationDeletedMessage;

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(
                    "ClientReservations",
                    "Clients",
                    new { area = "Admin", id = reservation.ClientId }
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

                TempData["Message"] = ValidationAndErrorMessageConstants.ReservationCreatedMessage;

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
