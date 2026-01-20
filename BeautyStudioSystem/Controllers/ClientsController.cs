using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Infrastructure.Contracts;
using BeautyStudioSystem.Infrastructure.Repository;
using BeautyStudioSystem.Services.Contracts;
using BeautyStudioSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BeautyStudioSystem.Controllers
{
    public class ClientsController : Controller
    {
        private IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            this._clientsService = clientsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clientsViewModels = await _clientsService.GetAllClientsAsync();
            return View(clientsViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            var clientsViewModels = await _clientsService.SearchClientsAsync(search);

            if (!clientsViewModels.Any())
            {
                ViewBag.Message = "No clients found.";
            }

                return View(clientsViewModels);
        }

        public async Task<IActionResult> ClientReservations(int id)
        {
            var reservationViewModels = await _clientsService.GetClientReservations(id);

            if (!reservationViewModels.Any())
            {
                ViewBag.Message = "Client doesn't have any reservations.";
            }

            var clientName = reservationViewModels.Select(rvm => rvm.ClientName).FirstOrDefault().ToString();
            ViewBag.ClientName = $"{clientName}";


            return View(reservationViewModels);
        }

        public async Task<IActionResult> DeleteClient(int id)
        {
            await _clientsService.DeleteClientAsync(id);

            TempData["Message"] = "Client deleted successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateClient(int id)
        {
            var client = await _clientsService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        public async Task<IActionResult> UpdateClient(ClientViewModel clientViewModel)
        {
            try
            {
                _clientsService.ValidateClient(clientViewModel);
                await _clientsService.UpdateClientAsync(clientViewModel);

                TempData["Message"] = "Client updated successfully.";
                return RedirectToAction("Index");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(clientViewModel);
            }
        }
    }
}
