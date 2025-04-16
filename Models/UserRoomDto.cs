using System.ComponentModel.DataAnnotations;

namespace MyProjectIT15.Models
{
    public class UserRoomDto
    {
        [Required]
        public int RoomId { get; set; }

        // Room can be optionally loaded if needed
        public Room? Room { get; set; }

        // AdminId will be set by the server, so it is not required in the form
        //public string AdminId { get; set; }

        //// Admin details are optional here
        //public User? Admin { get; set; }

        // TenantId is required for assigning tenant
        [Required]
        public string TenantId { get; set; }

        // Tenant details are optional here
        public User? Tenant { get; set; }

        // Required Start Date for the tenancy
        [Required]
        public DateTime StartDate { get; set; }

        // Required End Date for the tenancy
        [Required]
        public DateTime EndDate { get; set; }

        // Status of the room
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
