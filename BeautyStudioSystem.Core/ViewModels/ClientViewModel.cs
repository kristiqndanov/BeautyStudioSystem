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
        [StringLength(InputValidations.FullNameMaxLength), MinLength(InputValidations.FullNameMinLength, ErrorMessage = InputValidations.FullNameErrorMessage)]
        [RegularExpression(@"^[a-zA-Z]+ [a-zA-Z]+$", ErrorMessage = InputValidations.FullNameContainsTwoWordsMessage)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(InputValidations.EmailMaxLength)]
        [EmailAddress(ErrorMessage = InputValidations.InvalidEmailMessage)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(InputValidations.PhoneLength), MinLength(InputValidations.PhoneLength ,ErrorMessage = InputValidations.PhoneNumberErrorMessage)]
        
        public string Phone { get; set; } = null!;

        public string? UserId { get; set; }
        
        public string? CurrentRole { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
