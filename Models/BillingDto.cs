namespace MyProjectIT15.Models
{
    public class BillingDto
    {
        public string ?UserId { get; set; }
        public int MeterReadingId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
