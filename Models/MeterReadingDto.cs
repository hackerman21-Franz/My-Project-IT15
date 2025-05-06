using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectIT15.Models
{
    public class MeterReadingDto
    {
        public int? UserRoomId { get; set; }

        public int? RoomMeterId { get; set; }

        public DateTime ReadingDate { get; set; }
        public int PreviousReading { get; set; }
        public int CurrentReading { get; set; }
        public int Consumption { get; set; }

        public int WaterPreviousReading { get; set; }
        public int WaterCurrentReading { get; set; }
        public int WaterConsumption { get; set; }
        public string? UserId { get; set; }




        public List<SelectListItem> RoomMeters { get; set; } = new List<SelectListItem>(); // dropdown list
    }
}
