using System.ComponentModel.DataAnnotations;

namespace BeautyStudioSystem.Core.ViewModels
{
    public class CreateReservationFormModel
    {

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string Date { get; set; } = null!;

        [Required]
        public string StartTime { get; set; } = null!;
    }
}
