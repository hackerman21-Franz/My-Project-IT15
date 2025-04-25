using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class MeterReading
    {
        public int Id { get; set; }

        // Foreign key to the meter
        public int? MeterId { get; set; }

        // Navigation property to the meter
        [ForeignKey("MeterId")]
        public Meter? Meter { get; set; }

        // Foreign key to the Room
        public int? RoomId { get; set; }

        // Navigation property to the room
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }

        public DateTime ReadingDate { get; set; }
        public int PreviousReading { get; set; }
        public int CurrentReading { get; set; }
        public int Consumption { get; set; }

        // Foreign key to the Identity user
        public string? UserId { get; set; }

        // Navigation property to the ApplicationUser
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
