using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Phone { get; set; } = null!;


        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();


    }
}
