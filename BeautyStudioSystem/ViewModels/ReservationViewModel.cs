using BeautyStudioSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.ViewModels
{
    public class ReservationViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientName { get; set; } = null!;

        [Required]
        public string ServiceName { get; set; } = null!;

        [Required]
        public string Date { get; set; }

        [Required]
        public string StartTime { get; set; }
    }
}
