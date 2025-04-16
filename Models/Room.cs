using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Migrations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class Room
    {
        public int Id { get; set; }


        [MaxLength(100)]
        public string Room_Number { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Room_Type { get; set; } = string.Empty;

        [Precision(16, 2)]
        public decimal Monthly_Rent { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Foreign key to the Identity user
        public string? UserId { get; set; }

        // Navigation property to the ApplicationUser
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>();
    }
}
