using BeautyStudioSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyStudioSystem.Data.Seed
{
    internal static class DataToSeed
    {
        //Roles data
        internal static List<string> Roles => new List<string>()
        {
            "Admin",
            "Client",
            "Employee"
        };

        public const string AdminRole = "Admin";
        public const string ClientRole = "Client";
        public const string EmployeeRole = "Employee";

        //Admin data
        internal const string AdminEmail = "admin@admin.com";
        internal const string AdminPassword = "Admin123!";
        internal const string AdminPhone = "1234567890";
        internal const string AdminFirstName = "Admin";
        internal const string AdminLastName = "Admin";

        //Clients data
        internal static List<(string FirstName, string LastName, string Email, string Phone)> Clients => new List<(string FirstName, string LastName, string Email, string Phone)>
        {
            ("Vanya", "Petrova", "vanq.petrova@gmail.com", "0111223344"),
            ("Anna", "Vasileva", "anna.vasileva@abv.bg", "0123455555"),
            ("Kristina", "Hristova", "kristina.hristova@gmail.com", "0123456666")
        };

        internal const string ClientPassword = "Client123!";

        //ServiceCategories data
        internal static List<string> ServiceCategories => new List<string>()
        {
            "Hair",
            "Nails",
            "Face"
        };

        internal const string HairCategory = "Hair";
        internal const string NailsCategory = "Nails";
        internal const string FaceCategory = "Face";

        //Services data
        internal static List<(string Name, decimal Price, int DurationMinutes, string CategoryName)> Services = new List<(string Name, decimal Price, int DurationMinutes, string CategoryName)>
            {
                ("Haircut & Styling", 40.00m, 60, HairCategory),
                ("Manicure", 30.00m, 45, NailsCategory),
                ("Facial Treatment", 60.00m, 75, FaceCategory)
            };

        //Employees data
        internal const string EmployeePassword = "Employee123!";

    }
}
