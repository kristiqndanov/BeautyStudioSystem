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
    }
}
