using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult UserLog()
        {
            var userLogs = _context.UserLogs.ToList(); 
            return View(userLogs); 
        }

        public IActionResult UserManagement()
        {
            var users = _context.Users.Where(u=> !u.UserType.Equals("Yönetici")).ToList();
            return View(users);
        }

        public IActionResult WeatherManagement()
        {
            var weatherInfos = _context.WeatherInfos.ToList();
            return View(weatherInfos);
        }

        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("UserManagement");
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
            return RedirectToAction("WeatherManagement");
        }
    }

}
