using System.ComponentModel.DataAnnotations;

namespace MyProjectIT15.Models
{
	public class RoomMeterDto
	{
		[Required]
		public int RoomId { get; set; }

		[Required]
		public int MeterId { get; set; }
		public string Status { get; set; }
	}
}
