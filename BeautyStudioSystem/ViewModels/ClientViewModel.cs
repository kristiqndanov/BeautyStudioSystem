using BeautyStudioSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.ViewModels
{
    public class ClientViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(510), MinLength(3, ErrorMessage = "Full name must have at least 1 letter for first name and at least 1 letter for last name")]
        [RegularExpression(@"^[a-zA-Z]+ [a-zA-Z]+$", ErrorMessage = "Full Name must contain first name and last name separated by a space.")]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10), MinLength(10 ,ErrorMessage = "Phone number must be exactly 10 digits")]
        
        public string Phone { get; set; } = null!;


        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
