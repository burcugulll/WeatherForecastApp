using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApp.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required]
        public string Name { get; set; }
        [MinLength(5)]
        public string Password { get; set; }
        [MinLength(5)]
        public string PasswordAgain { get; set; }
        //[MinLength(2)]
        //public string DefaultCityName { get; set; }
    }
}
