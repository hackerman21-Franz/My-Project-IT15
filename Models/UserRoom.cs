using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class UserRoom
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }

        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public User? Admin { get; set; }

        public string? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public User? Tenant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
