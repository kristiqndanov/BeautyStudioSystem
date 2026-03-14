using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Core.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
        public string? UserId { get; set; }
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
    }
}