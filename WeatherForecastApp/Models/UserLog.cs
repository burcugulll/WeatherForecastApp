using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApp.Models
{
    public class UserLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string Username { get; set; }

        public DateTime LogTime { get; set; }

        public string IPAddress { get; set; }

        public string Log { get; set; }
    }
}
