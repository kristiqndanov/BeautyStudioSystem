using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Data.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; } = null!;

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
