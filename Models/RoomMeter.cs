using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
	public class RoomMeter
	{
		public int Id { get; set; }
        public int RoomId { get; set; }
        // Navigation property to the room
        [ForeignKey("RoomId")]
		public Room? Room { get; set; }

        public int MeterId { get; set; }
        // Navigation property to the meter
        [ForeignKey("MeterId")]
		public Meter? Meter { get; set; }

		public DateTime installedDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
	}
}
