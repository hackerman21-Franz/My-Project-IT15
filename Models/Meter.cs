using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyProjectIT15.Models
{
    public class Meter
    {
        public int Id { get; set; }


        [MaxLength(100)]
        public string Meter_Number { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(100)]
        public string MeterType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }



        // Foreign key to the Identity user
        public string? UserId { get; set; }

        // Navigation property to the ApplicationUser
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<RoomMeter> RoomMeters { get; set; } = new List<RoomMeter>();
    }
}
