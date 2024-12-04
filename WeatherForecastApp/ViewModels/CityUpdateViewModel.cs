using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApp.ViewModels
{
    public class CityUpdateViewModel
    {
        [MinLength(2)]
        public string DefaultCityName { get; set; }
    }
}
