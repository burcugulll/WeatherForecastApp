using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApp.Models
{
    public class WeatherInfo
    {
        [Key]
        public int WeatherInfoId { get; set; }

        [Required]
        public DateTime WeatherDate { get; set; }

        [Required]
        public string CityName { get; set; }

        public float Temperature { get; set; }

        public string MainStatus { get; set; }
        public string Icon { get; set; } 


    }
}
