using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class MeterReading
    {
        public int Id { get; set; }

		// Foreign key to the Room
		public int? UserRoomId { get; set; }

		// Navigation property to the room
		[ForeignKey("UserRoomId")]
		public UserRoom? UserRoom { get; set; }

		// Foreign key to the meter
		public int? RoomMeterId { get; set; }

        // Navigation property to the meter
        [ForeignKey("RoomMeterId")]
        public RoomMeter? RoomMeter { get; set; }

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
