using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Infrastructure.Repository;
using BeautyStudioSystem.Services.Contracts;
using BeautyStudioSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Controllers
{
    public class ClientsController : Controller
    {
        private IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            this._clientsService = clientsService;
        }

        public async Task<IActionResult> Index()
        {
            var clientsViewModels = await _clientsService.GetAllClientsAsync();
            return View(clientsViewModels);
        }

        public async Task<IActionResult> ClientReservations(int id)
        {
            

            var reservationViewModels = await _clientsService.GetClientReservations(id);
            var clientName = reservationViewModels.Select(rvm => rvm.ClientName).FirstOrDefault().ToString();
            ViewBag.ClientName = $"{clientName}";


            return View(reservationViewModels);
        }
    }
}
