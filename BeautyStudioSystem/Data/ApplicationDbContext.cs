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

        }
    }
}

