using BeautyStudioSystem.Data.Models;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Infrastructure.Repository;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeautyStudioSystem.Controllers
{
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientsController(IClientsService clientsService, IEmployeeRepository employeeRepository, UserManager<IdentityUser> userManager)
        {
            this._clientsService = clientsService;
            this._userManager = userManager;
            this._employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var clientsViewModels = await _clientsService.GetAllClientsAsync();
            return View(clientsViewModels);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string search)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var clientsViewModels = await _clientsService.SearchClientsAsync(search);

                if (!clientsViewModels.Any())
                {
                    ViewBag.Message = "No clients found.";
                }

                return View(clientsViewModels);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClientReservations(int id)
        {
            var reservationViewModels = await _clientsService.GetClientReservations(id);

            if (!reservationViewModels.Any())
            {
                ViewBag.Message = "Client doesn't have any reservations.";
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var clientViewModel = await _clientsService.GetClientByIdAsync(id);

           if (clientViewModel != null)
            {
                await _clientsService.DeleteClientAsync(id);

                TempData["Message"] = "Client deleted successfully.";
            }

           

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClient(int id)
        {
            var client = await _clientsService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(client.UserId))
            {
                var user = await _userManager.FindByIdAsync(client.UserId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    client.CurrentRole = roles.FirstOrDefault() ?? "Client";
                }
            }

            return View(client);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClient(ClientViewModel clientViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(clientViewModel);
            }

            await _clientsService.UpdateClientAsync(clientViewModel);
            TempData["Message"] = "Client updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToEmployee(ClientViewModel clientViewModel)
        {
            var user = await _userManager.FindByIdAsync(clientViewModel.UserId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Employee");

            var names = clientViewModel.FullName.Split(' ', 2);
            await _employeeRepository.AddEmployeeAsync(new Employee
            {
                FirstName = names[0],
                LastName = names.Length > 1 ? names[1] : string.Empty,
                Email = clientViewModel.Email,
                Phone = clientViewModel.Phone,
                UserId = clientViewModel.UserId
            });

            await _clientsService.SoftDeleteClientAsync(clientViewModel.Id);
            TempData["Message"] = $"{clientViewModel.FullName} is now an Employee.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);


            var clientId = await _clientsService.GetClientIdByUserId(user.Id);

            var reservationViewModels = await _clientsService.GetClientReservations(clientId);

            if (!reservationViewModels.Any())
            {
                ViewBag.Message = "You do not have any reservations yet.";
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
