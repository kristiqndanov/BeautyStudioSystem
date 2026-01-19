using BeautyStudioSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeautyStudioSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.ClientId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.ServiceId);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ServiceId, r.Date, r.StartTime })
                .IsUnique();


            modelBuilder.Entity<Client>().HasData(
        new Client
        {
            Id = 1,
            FirstName = "Anna",
            LastName = "Petrova",
            Email = "anna.petrova@gmail.com",
            Phone = "0888123456"
        },
        new Client
        {
            Id = 2,
            FirstName = "Maria",
            LastName = "Ivanova",
            Email = "maria.ivanova@gmail.com",
            Phone = "0888234567"
        },
        new Client
        {
            Id = 3,
            FirstName = "Elena",
            LastName = "Georgieva",
            Email = "elena.georgieva@gmail.com",
            Phone = "0888345678"
        }
    );

            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    Name = "Haircut & Styling",
                    Price = 40.00m
                },
                new Service
                {
                    Id = 2,
                    Name = "Manicure",
                    Price = 30.00m
                },
                new Service
                {
                    Id = 3,
                    Name = "Facial Treatment",
                    Price = 60.00m
                }
            );

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    Id = 1,
                    ClientId = 1,
                    ServiceId = 1,
                    Date = new DateTime(2026, 2, 1),
                    StartTime = new DateTime(2026, 2, 1, 10, 0, 0)
                },
                new Reservation
                {
                    Id = 2,
                    ClientId = 2,
                    ServiceId = 2,
                    Date = new DateTime(2026, 2, 2),
                    StartTime = new DateTime(2026, 2, 2, 12, 0, 0)
                },
                new Reservation
                {
                    Id = 3,
                    ClientId = 3,
                    ServiceId = 3,
                    Date = new DateTime(2026, 2, 3),
                    StartTime = new DateTime(2026, 2, 3, 15, 0, 0)
                });

        }
    }
}

