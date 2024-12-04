using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name; 

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User updatedUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.UserId == updatedUser.UserId);
                if (user != null)
                {
                    user.Name = updatedUser.Name;

                    user.Password = updatedUser.HashPassword(updatedUser.Password); 

                    user.DefaultCityName = updatedUser.DefaultCityName;

                    _context.SaveChanges();
                }
                return RedirectToAction("Index","Profile");
            }
            return View(updatedUser);
        }
    }
}
