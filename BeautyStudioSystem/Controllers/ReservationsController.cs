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
    }
}
