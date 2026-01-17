using BeautyStudioSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.ViewModels
{
    public class ClientViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(400)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Phone { get; set; } = null!;


        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
