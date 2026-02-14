using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BeautyStudioSystem.Controllers
{
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;
        private readonly IServicesService _servicesService;

        public ReservationsController(IReservationsService reservationsService, IServicesService servicesService)
        {
            _reservationsService = reservationsService;
            _servicesService = servicesService;
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

            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }

        }

    }
}
