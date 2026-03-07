using BeautyStudioSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Core.ViewModels
{
    public class ReservationViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientName { get; set; } = null!;

        [Required]
        public int ClientId { get; set; }

        [Required]
        public string ServiceName { get; set; } = null!;

        [Required]
        public string EmployeeName { get; set; } = null!; 

        [Required]
        public string Date { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }
    }
}
