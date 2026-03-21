using BeautyStudioSystem.Common;
using BeautyStudioSystem.Core.Services.Contracts;
using BeautyStudioSystem.Core.ViewModels;
using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [AutoValidateAntiforgeryToken]
    public class ClientsController : Controller
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
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 20;
            var result = await _clientsService.GetClientsPagedAsync(search, page, pageSize);
            ViewBag.Search = search;
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClientReservations(int id)
        {
            var reservationViewModels = await _clientsService.GetClientReservations(id);

            if (!reservationViewModels.Any())
            {
                ViewBag.Message = ValidationAndErrorMessageConstants.NoClientReservationsMessage;
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

                TempData["Message"] = ValidationAndErrorMessageConstants.ClientDeletedMessage;
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
            TempData["Message"] = ValidationAndErrorMessageConstants.ClientUpdatedMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToEmployee(ClientViewModel clientViewModel)
        {
            var user = await _userManager.FindByIdAsync(clientViewModel.UserId);
            if (user == null)
            {
                TempData["Error"] = ValidationAndErrorMessageConstants.UserNotFoundMessage;
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
            TempData["Message"] = string.Format(ValidationAndErrorMessageConstants.PromoteToEmployeeMessage, clientViewModel.FullName);
            return RedirectToAction("Index");
        }

    }
}