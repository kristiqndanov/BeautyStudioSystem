using BeautyStudioSystem.Common;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IReservationsService _reservationsService;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(
            IReservationsService reservationsService,
            UserManager<IdentityUser> userManager)
        {
            _reservationsService = reservationsService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var reservations = await _reservationsService.GetReservationsByEmployeeAsync(user.Id);
            if (!reservations.Any())
            {
                ViewBag.Message = ValidationAndErrorMessageConstants.NoReservationsMessage;
            }
            return View(reservations);
        }

        
    }
}