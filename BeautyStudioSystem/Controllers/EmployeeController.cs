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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IServiceCategoryRepository _serviceCategoryRepository;

        public EmployeeController(
            IReservationsService reservationsService,
            UserManager<IdentityUser> userManager,
            IEmployeeRepository employeeRepository,
            IClientsRepository clientsRepository,
            IServiceCategoryRepository serviceCategoryRepository)
        {
            _reservationsService = reservationsService;
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _clientsRepository = clientsRepository;
            _serviceCategoryRepository = serviceCategoryRepository;
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                Email = e.Email,
                Phone = e.Phone,
                UserId = e.UserId
            });
            return View(employeeViewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EmployeeReservations(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            var reservations = await _reservationsService.GetReservationsByEmployeeAsync(employee.UserId);

            if (!reservations.Any())
            {
                ViewBag.Message = ValidationAndErrorMessageConstants.EmployeeHasNoReservationsMessage;
            }

            ViewBag.EmployeeName = $"{employee.FirstName} {employee.LastName}";
            return View(reservations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            var allCategories = await _serviceCategoryRepository.GetAllAsync();

            var employeeViewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Phone = employee.Phone,
                UserId = employee.UserId,
                SelectedCategoryIds = employee.ServiceCategory.Select(c => c.Id).ToList()
            };

            ViewBag.AllCategories = allCategories;
            return View(employeeViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditEmployee(EmployeeViewModel employeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllCategories = await _serviceCategoryRepository.GetAllAsync();
                return View(employeeViewModel);
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeViewModel.Id);
            if (employee == null) return NotFound();

            var names = employeeViewModel.FullName.Split(' ', 2);
            employee.FirstName = names[0];
            employee.LastName = names.Length > 1 ? names[1] : string.Empty;
            employee.Email = employeeViewModel.Email;
            employee.Phone = employeeViewModel.Phone;

            var allCategories = await _serviceCategoryRepository.GetAllAsync();
            employee.ServiceCategory = allCategories
                .Where(c => employeeViewModel.SelectedCategoryIds.Contains(c.Id))
                .ToList();

            await _employeeRepository.UpdateEmployee(employee);
            TempData["Message"] = ValidationAndErrorMessageConstants.EmployeeUpdatedMessage;
            return RedirectToAction("AdminIndex");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevertToClient(EmployeeViewModel employeeViewModel)
        {
            var user = await _userManager.FindByIdAsync(employeeViewModel.UserId);
            if (user == null)
            {
                TempData["Error"] = ValidationAndErrorMessageConstants.UserNotFoundMessage;
                return RedirectToAction("AdminIndex");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Client");

            var employee = await _employeeRepository.GetByIdAsync(employeeViewModel.Id);
            if (employee != null)
            {
                await _employeeRepository.DeleteEmployee(employee);
            }

            
            var names = employeeViewModel.FullName.Split(' ', 2);
            await _clientsRepository.AddClientAsync(new Client
            {
                FirstName = names[0],
                LastName = names.Length > 1 ? names[1] : string.Empty,
                Email = employeeViewModel.Email,
                Phone = employeeViewModel.Phone,
                UserId = employeeViewModel.UserId
            });

            TempData["Message"] = string.Format(ValidationAndErrorMessageConstants.RevertToClientMessage, employeeViewModel.FullName);
            return RedirectToAction("AdminIndex");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee != null)
            {
                await _employeeRepository.DeleteEmployee(employee);
                var user = await _userManager.FindByIdAsync(employee.UserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            TempData["Message"] = ValidationAndErrorMessageConstants.EmployeeDeletedMessage;
            return RedirectToAction("AdminIndex");
        }
    }
}