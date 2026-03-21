using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BeautyStudioSystem.Common;

namespace BeautyStudioSystem.Controllers
{
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientsController(IClientsService clientsService, UserManager<IdentityUser> userManager)
        {
            this._clientsService = clientsService;
            this._userManager = userManager;
        }



        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);


            var clientId = await _clientsService.GetClientIdByUserId(user.Id);

            var reservationViewModels = await _clientsService.GetClientReservations(clientId);

            if (!reservationViewModels.Any())
            {
                ViewBag.Message = ValidationAndErrorMessageConstants.NoCurrentReservationsMessage;
            }

            if (reservationViewModels.Any())
            {
                ViewBag.ClientName = reservationViewModels.First().ClientName;
            }
            else
            {
                ViewBag.ClientName = "Client";
            }

            return View(reservationViewModels);
        }

    }
}
