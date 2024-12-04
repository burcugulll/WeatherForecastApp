using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Linq; 
using WeatherForecastApp.Models;
using WeatherForecastApp.Services;
using WeatherForecastApp.ViewModels;

namespace WeatherForecastApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private WeatherService _weatherService;
        public HomeController(ApplicationDbContext context,WeatherService weatherService)  
        {
            _context = context;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var countryClaim = User.Claims.FirstOrDefault(c => c.Type == "City");
            string countryName = countryClaim?.Value;
            var adminAddedCities = _context.WeatherInfos.Select(w => w.CityName).Distinct().ToList();
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user != null && adminAddedCities.Contains(user.DefaultCityName))
            {
                WeatherInfo response = await _weatherService.GetWeatherFromApi(countryName.IsNullOrEmpty() ? "Denizli" : countryName, DateTime.Now);

                TempData["temp"] = response.Temperature;
                TempData["city"] = response.CityName;
                TempData["iconUrl"] = response.Icon;
                TempData["mainStatus"] = response.MainStatus;
                TempData["weatherDate"] = response.WeatherDate;

                var weatherInfos = _context.WeatherInfos
                    .Where(w => w.CityName == user.DefaultCityName)
                    .ToList();

                var weeklyWeatherInfos = weatherInfos
                    .Where(w => w.WeatherDate >= DateTime.Now.Date.AddDays(-7))
                    .OrderBy(w => w.WeatherDate)
                    .ToList();

                ViewBag.WeeklyWeatherInfos = weeklyWeatherInfos;

                return View(weatherInfos);
            }
            else
            {
                TempData["temp"] = null;
                TempData["city"] = user.DefaultCityName;
                TempData["iconUrl"] = null;
                TempData["mainStatus"] = null;

                return View(); 
            }
        }

        
        [HttpPost]
        public IActionResult UpdateCity(string DefaultCityName)
        {
            if (string.IsNullOrWhiteSpace(DefaultCityName))
            {
                TempData["ErrorMessage"] = "Þehir adý boþ olamaz.";
                return RedirectToAction("Index");
            }

            var username = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            if (user != null)
            {
                if (user.DefaultCityName == DefaultCityName)
                {
                    TempData["SuccessMessage"] = "Þehir adý zaten ayný, güncellenemedi.";
                }
                else
                {
                    user.DefaultCityName = DefaultCityName;
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Þehir adý baþarýyla güncellendi.";
                    TempData["city"] = DefaultCityName;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
            }

            return RedirectToAction("Index"); 
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
