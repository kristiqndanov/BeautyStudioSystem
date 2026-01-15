using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyStudioSystem.Data.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public Service? Service { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime StartTime { get; set; }


    }
}
