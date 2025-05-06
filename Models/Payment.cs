using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int BillingId { get; set; }
        [ForeignKey("BillingId")]
        public Billing? Billing { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPaid { get; set; }

        public string CheckoutSessionId { get; set; } 

        public string? ReferenceNumber { get; set; }

        public string? LastWebhookEvent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedAt { get; set; }

        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
    }
}
