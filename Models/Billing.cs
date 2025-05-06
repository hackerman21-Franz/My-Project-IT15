using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class Billing
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        // Navigation property to the ApplicationUser
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? MeterReadingId { get; set; }

        // Navigation property to the MeterReading
        [ForeignKey("MeterReadingId")]
        public MeterReading? MeterReading { get; set; }

        public DateTime ReadingDate { get; set; }
        public DateTime DueDate { get; set; }

        //public decimal PreviousBalance { get; set; }
        //public decimal CurrentBalance { get; set; }
        
        [Precision(16, 2)]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
