using System.ComponentModel.DataAnnotations;

namespace MyProjectIT15.Models
{
    public class MeterDto
    {
        [Required]
        public string Meter_Number { get; set; } = string.Empty;

        [Required]
        public string MeterType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
