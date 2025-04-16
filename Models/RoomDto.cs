using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyProjectIT15.Models
{
    public class RoomDto
    {
        [Required, MaxLength(100)]
        public string Room_Number { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Room_Type { get; set; } = string.Empty;
        [Required]
        public decimal Monthly_Rent { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required, MaxLength(100)]
        public string Status { get; set; } = string.Empty;
    }
}
