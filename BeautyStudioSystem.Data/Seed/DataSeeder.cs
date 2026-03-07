using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BeautyStudioSystem.Data.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager, services.GetRequiredService<IClientsRepository>());
            await SeedClientsAsync(userManager, services.GetRequiredService<IClientsRepository>());
            await SeedEmployeesAsync(userManager, services.GetRequiredService<IEmployeeRepository>());
            await SeedServiceCategoriesAsync(services.GetRequiredService<IServiceCategoryRepository>());
            await SeedServicesAsync(services.GetRequiredService<IServicesRepository>(), services.GetRequiredService<IServiceCategoryRepository>());
            await SeedReservationsAsync(
                services.GetRequiredService<IReservationsRepository>(),
                services.GetRequiredService<IClientsRepository>(),
                services.GetRequiredService<IServicesRepository>(),
                services.GetRequiredService<IEmployeeRepository>());
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Client", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, IClientsRepository clientsRepository)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) != null) return;

            var user = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                PhoneNumber = "0123456789",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");

                await clientsRepository.AddClientAsync(new Client
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = adminEmail,
                    Phone = "0123456789",
                    UserId = user.Id
                });
            }
        }

        private static async Task SeedClientsAsync(UserManager<IdentityUser> userManager, IClientsRepository clientsRepository)
        {
            var clientsData = new List<(string FirstName, string LastName, string Email, string Phone)>
            {
                ("Vanya", "Petrova", "vanq.petrova@gmail.com", "0111223344"),
                ("Anna", "Vasileva", "anna.vasileva@abv.bg", "0123455555"),
                ("Kristina", "Hristova", "kristina.hristova@gmail.com", "0123456666")
            };

            foreach (var c in clientsData)
            {
                if (await userManager.FindByEmailAsync(c.Email) != null) continue;

                var user = new IdentityUser
                {
                    UserName = c.Email,
                    Email = c.Email,
                    PhoneNumber = c.Phone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Client123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Client");

                    await clientsRepository.AddClientAsync(new Client
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        Phone = c.Phone,
                        UserId = user.Id
                    });
                }
            }
        }

        private static async Task SeedEmployeesAsync(UserManager<IdentityUser> userManager, IEmployeeRepository employeeRepository)
        {
            var employeesData = new List<(string FirstName, string LastName, string Email, string Phone)>
            {
                ("Maria", "Todorova", "maria.todorova@beautystudio.com", "0888111222"),
                ("Elena", "Georgieva", "elena.georgieva@beautystudio.com", "0888333444")
            };

            foreach (var e in employeesData)
            {
                if (await userManager.FindByEmailAsync(e.Email) != null) continue;

                var user = new IdentityUser
                {
                    UserName = e.Email,
                    Email = e.Email,
                    PhoneNumber = e.Phone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Employee123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Employee");

                    await employeeRepository.AddEmployeeAsync(new Employee
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Phone = e.Phone,
                        Email = e.Email,
                        UserId = user.Id
                    });
                }
            }
        }

        private static async Task SeedServiceCategoriesAsync(IServiceCategoryRepository serviceCategoryRepository)
        {
            var categories = new List<string> { "Hair", "Nails", "Face" };

            var existing = await serviceCategoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                if (existing.Any(c => c.Name == category)) continue;

                await serviceCategoryRepository.AddServiceCategoryAsync(new ServiceCategory
                {
                    Name = category
                });
            }
        }

        private static async Task SeedServicesAsync(IServicesRepository servicesRepository, IServiceCategoryRepository serviceCategoryRepository)
        {
            var allServices = await servicesRepository.GetAllAsync();
            var allCategories = await serviceCategoryRepository.GetAllAsync();

            var servicesData = new List<(string Name, decimal Price, int DurationMinutes, string CategoryName)>
            {
                ("Haircut & Styling", 40.00m, 60, "Hair"),
                ("Manicure", 30.00m, 45, "Nails"),
                ("Facial Treatment", 60.00m, 75, "Face")
            };

            foreach (var s in servicesData)
            {
                if (allServices.Any(x => x.Name == s.Name)) continue;

                var category = allCategories.FirstOrDefault(c => c.Name == s.CategoryName);
                if (category == null) continue;

                await servicesRepository.AddServiceAsync(new Service
                {
                    Name = s.Name,
                    Price = s.Price,
                    Duration = s.DurationMinutes,
                    ServiceCategoryId = category.Id
                });
            }
        }

        private static async Task SeedReservationsAsync(
            IReservationsRepository reservationsRepository,
            IClientsRepository clientsRepository,
            IServicesRepository servicesRepository,
            IEmployeeRepository employeeRepository)
        {
            var allReservations = await reservationsRepository.GetAllAsync();
            var allServices = await servicesRepository.GetAllAsync();
            var allEmployees = await employeeRepository.GetAllAsync();

            if (allReservations.Any()) return;

            var reservationsData = new List<(int ClientId, int ServiceId, int EmployeeIndex, DateTime Date, DateTime StartTime)>
            {
                (2, 1, 0, new DateTime(2026, 4, 1), new DateTime(2026, 4, 1, 10, 0, 0)),
                (3, 2, 1, new DateTime(2026, 4, 2), new DateTime(2026, 4, 2, 12, 0, 0)),
                (4, 3, 0, new DateTime(2026, 4, 3), new DateTime(2026, 4, 3, 15, 0, 0))
            };

            var employeeList = allEmployees.ToList();
            var serviceList = allServices.ToList();

            foreach (var r in reservationsData)
            {
                var service = serviceList.FirstOrDefault(s => s.Id == r.ServiceId);
                var employee = employeeList.ElementAtOrDefault(r.EmployeeIndex);

                if (service == null || employee == null) continue;

                var endTime = r.StartTime.AddMinutes(service.Duration);

                await reservationsRepository.AddReservationAsync(new Reservation
                {
                    ClientId = r.ClientId,
                    ServiceId = r.ServiceId,
                    EmployeeId = employee.Id,
                    Date = r.Date,
                    StartTime = r.StartTime,
                    EndTime = endTime
                });
            }
        }
    }
}