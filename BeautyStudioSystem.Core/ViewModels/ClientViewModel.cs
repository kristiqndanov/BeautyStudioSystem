using BeautyStudioSystem.Data.Models;
using System.ComponentModel.DataAnnotations;
using BeautyStudioSystem.Core.Common;


namespace BeautyStudioSystem.Core.ViewModels
{
    public class ClientViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(InputValidations.FullNameMaxLength), MinLength(InputValidations.FullNameMinLength, ErrorMessage = "Full name must have at least 1 letter for first name and at least 1 letter for last name")]
        [RegularExpression(@"^[a-zA-Z]+ [a-zA-Z]+$", ErrorMessage = "Full Name must contain first name and last name separated by a space.")]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(InputValidations.EmailMaxLength)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(InputValidations.PhoneLength), MinLength(InputValidations.PhoneLength ,ErrorMessage = "Phone number must be exactly 10 digits")]
        
        public string Phone { get; set; } = null!;

        public string? UserId { get; set; }
        
        public string? CurrentRole { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
