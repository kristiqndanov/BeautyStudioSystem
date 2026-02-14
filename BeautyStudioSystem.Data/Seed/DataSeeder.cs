using BeautyStudioSystem.Data.Infrastructure.Contracts;
using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await SeedServicesAsync(services.GetRequiredService<IServicesRepository>());
            await SeedReservationsAsync(services.GetRequiredService<IReservationsRepository>(), services.GetRequiredService<IClientsRepository>(), services.GetRequiredService<IServicesRepository>());

        }
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            List<string> roles = new List<string>();
            roles.Add("Admin");
            roles.Add("Client");

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
            string adminFirstName = "admin";
            string adminLastName = "admin";
            string adminEmail = "admin@admin.com";
            string adminPhone = "0123456789";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    PhoneNumber = adminPhone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");


                    var admin = new Client
                    {
                        FirstName = adminFirstName,
                        LastName = adminLastName,
                        Email = adminEmail,
                        Phone = adminPhone,
                        UserId = user.Id,

                    };

                    await clientsRepository.AddClientAsync(admin);
                }
            }
        }

        private static async Task SeedClientsAsync(UserManager<IdentityUser> userManager, IClientsRepository clientsRepository)
        {

            var clientsData = new List<(string FirstName, string LastName, string Email, string Phone)>
            {
                ("Vanq", "Petrova", "vanq.petrova@gmail.com", "0111223344"),
                ("Anna", "Vasileva", "anna.vasileva@abv.bg", "0123455555"),
                ("Kristina", "Hristova", "kristina.hristova@gmail.com", "0123456666")
            };

            string clientPassword = "Client123!";

            foreach (var clientInfo in clientsData)
            {
                var existingUser = await userManager.FindByEmailAsync(clientInfo.Email);

                if (existingUser != null)
                {
                    continue;
                }

                var user = new IdentityUser
                {
                    UserName = clientInfo.Email,
                    Email = clientInfo.Email,
                    PhoneNumber = clientInfo.Phone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, clientPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Client");

                    var client = new Client
                    {
                        FirstName = clientInfo.FirstName,
                        LastName = clientInfo.LastName,
                        Email = clientInfo.Email,
                        Phone = clientInfo.Phone,
                        UserId = user.Id
                    };

                    await clientsRepository.AddClientAsync(client);
                }
            }
        }

        private static async Task SeedServicesAsync(IServicesRepository servicesRepository)
        {
            var servicesData = new List<(string Name, decimal Price)>
            {
                ("Haircut & Styling", 40.00m),
                ("Manicure", 30.00m),
                ("Facial Treatment", 60.00m)
            };

            foreach (var serviceInfo in servicesData)
            {
                var allServices = await servicesRepository.GetAllAsync();
                var existingService = allServices.FirstOrDefault(s => s.Name == serviceInfo.Name);

                if (existingService != null)
                {
                    continue;
                }

                var service = new Service
                {
                    Name = serviceInfo.Name,
                    Price = serviceInfo.Price
                };

                await servicesRepository.AddServiceAsync(service);
            }
        }

        private static async Task SeedReservationsAsync(IReservationsRepository reservationsRepository, IClientsRepository clientsRepository, IServicesRepository servicesRepository)
        {
            var reservationsData = new List<(int ClientId, int ServiceId, DateTime Date, DateTime StartTime)>
            {
                (2, 1, new DateTime(2026, 2, 1), new DateTime(2026, 2, 1, 10, 0, 0)),
                (3, 2, new DateTime(2026, 2, 2), new DateTime(2026, 2, 2, 12, 0, 0)),
                (4, 3, new DateTime(2026, 2, 3), new DateTime(2026, 2, 3, 15, 0, 0))
            };

            foreach (var reservationInfo in reservationsData)
            {
                var allReservations = await reservationsRepository.GetAllAsync();
                var existingReservation = allReservations.FirstOrDefault(r =>
                    r.ClientId == reservationInfo.ClientId &&
                    r.ServiceId == reservationInfo.ServiceId &&
                    r.Date == reservationInfo.Date &&
                    r.StartTime == reservationInfo.StartTime);

                if (existingReservation != null)
                {
                    continue;
                }

                var reservation = new Reservation
                {
                    ClientId = reservationInfo.ClientId,
                    ServiceId = reservationInfo.ServiceId,
                    Date = reservationInfo.Date,
                    StartTime = reservationInfo.StartTime
                };

                await reservationsRepository.AddReservationAsync(reservation);
            }
        }
    }
}