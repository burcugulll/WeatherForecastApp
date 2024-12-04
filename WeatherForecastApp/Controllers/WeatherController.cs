using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Models;
using WeatherForecastApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherForecastApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WeatherService _weatherService;

        public WeatherController(ApplicationDbContext context, WeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        public IActionResult Index()
        {
            var weatherInfos = _context.WeatherInfos.OrderBy(w => w.WeatherDate).ToList();
            return View(weatherInfos);
        }
        [HttpGet]
        public IActionResult FilterWeatherInfo(string cityName, DateTime? weatherDate)
        {
            var weatherInfos = _context.WeatherInfos.AsQueryable();

            if (!string.IsNullOrEmpty(cityName))
            {
                weatherInfos = weatherInfos.Where(w => w.CityName.Contains(cityName));
            }

            if (weatherDate.HasValue)
            {
                weatherInfos = weatherInfos.Where(w => w.WeatherDate.Date == weatherDate.Value.Date);
            }

            var filteredWeatherInfos = weatherInfos.OrderBy(w => w.WeatherDate).ToList();
            return View("Index", filteredWeatherInfos);

            //return RedirectToAction("WeatherManagement", "Admin", new { weatherInfos = filteredWeatherInfos });
        }

        [HttpPost]
        public async Task<IActionResult> AddWeatherInfo(string cityName, DateTime weatherDate)
        {
            if (ModelState.IsValid)
            {
                var weatherInfo = await _weatherService.GetWeatherFromApi(cityName, weatherDate);
                if (weatherInfo != null)
                {
                    _context.WeatherInfos.Add(weatherInfo);
                    await _context.SaveChangesAsync();  
                }
                else
                {
                    ModelState.AddModelError("", "Hava durumu bilgisi alınamadı.");
                }
            }
            return RedirectToAction("WeatherManagement", "Admin");
        }

        [HttpPost]
        public IActionResult DeleteWeatherInfo(int weatherInfoId)
        {
            var weatherInfo = _context.WeatherInfos.SingleOrDefault(w => w.WeatherInfoId == weatherInfoId);
            if (weatherInfo != null)
            {
                _context.WeatherInfos.Remove(weatherInfo);
                _context.SaveChanges();
            }
            return RedirectToAction("WeatherManagement", "Admin");
        }
    }
}