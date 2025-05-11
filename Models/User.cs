using Microsoft.AspNetCore.Identity;

namespace MyProjectIT15.Models
{
    public class User : IdentityUser 
    {
        public String FirstName { get; set; } = "";
        public String MiddleName { get; set; } = "";
        public String LastName { get; set; } = "";
        public String Address { get; set; } = "";


        public DateTime CreatedAt { get; set; }


        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<UserRoom> AssignedAsAdmin { get; set; } = new List<UserRoom>();
        public ICollection<UserRoom> AssignedAsTenant { get; set; } = new List<UserRoom>();

        public ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

        public ICollection<Billing> Billings { get; set; } = new List<Billing>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
