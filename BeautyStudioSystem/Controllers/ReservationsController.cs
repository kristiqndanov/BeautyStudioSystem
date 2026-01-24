using BeautyStudioSystem.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
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
                return NotFound();

            await _reservationsService.DeleteReservation(id);

            TempData["Message"] = "Reservation deleted successfully.";

            return RedirectToAction(
                "ClientReservations",   
                "Clients",             
                new { id = reservation.ClientId }
            );
        }

    }
}
