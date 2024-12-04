using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WeatherForecastApp.Models;
using Microsoft.AspNetCore.Authentication;
using WeatherForecastApp.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace WeatherForecastApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user != null)
            {
                if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.Now)
                {
                    ViewBag.Message = "Çok sayıda hatalı giriş denemesi yaptınız. Lütfen 1 dakika sonra tekrar deneyin.";
                    return View();
                }

                var hashedPassword = user.HashPassword(password);
                if (user.Password == hashedPassword)
                {
                    user.LoginAttempts = 0;
                    user.LockoutEndTime = null;

                    var userLog = new UserLog
                    {
                        Username = username,
                        LogTime = DateTime.Now,
                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                        Log = "Başarılı giriş"
                    };
                    _context.UserLogs.Add(userLog);
                    _context.SaveChanges();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.UserType),
                        new Claim("City", user.DefaultCityName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "login");

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                    if (user.UserType == "Yönetici")
                    {

                        return RedirectToAction("UserManagement", "Admin");
                    }
                    else
                    {

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    user.LoginAttempts++;
                    var userLog = new UserLog
                    {
                        Username = username,
                        LogTime = DateTime.Now,
                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                        Log = "Başarısız giriş"
                    };
                    _context.UserLogs.Add(userLog);
                    _context.SaveChanges();
                    if (user.LoginAttempts >= 3)
                    {
                        user.LockoutEndTime = DateTime.Now.AddMinutes(1);
                        ViewBag.Message = "3 kez yanlış şifre girdiniz. 1 dakika boyunca giriş yapamazsınız.";
                    }
                    else
                    {
                        ViewBag.Message = $"Geçersiz kullanıcı adı veya şifre! {3 - user.LoginAttempts} hakkınız kaldı.";
                    }
                    _context.SaveChanges();
                }
            }
            else
            {
                ViewBag.Message = "Geçersiz kullanıcı adı veya şifre!";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account"); 
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAdminUser()
        {
            string salt = WeatherForecastApp.Models.User.GenerateSalt();
            string hashedPassword = new User { Salt = salt }.HashPassword("admin123"); // Şifre: admin123

            var adminUser = new User
            {
                UserName = "admin",
                Password = hashedPassword,
                Salt = salt,
                Name = "Admin User",
                UserType = "Yönetici",
                DefaultCityName = "İstanbul",
                Status = "Active",
                LoginAttempts = 0,
                LockoutEndTime = null
            };

            _context.Users.Add(adminUser);
            _context.SaveChanges();

            return Content("Yönetici kullanıcı başarıyla oluşturuldu!");
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Name = model.Name,
                    DefaultCityName = model.DefaultCityName,
                    UserType = "Son Kullanıcı", 
                    Status = "Active" 
                };

                string salt = WeatherForecastApp.Models.User.GenerateSalt();
                user.Salt = salt;
                user.Password = user.HashPassword(model.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = User.Identity.Name; 

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var user = _context.Users.SingleOrDefault(u => u.UserName == username); 
            if (user == null)
            {
                return NotFound(); 
            }

            return View(user); 
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            
            var model = new UserUpdateViewModel
            {
                Name = user.Name,
                //DefaultCityName = user.DefaultCityName
            };

            return View(model); 
        }
        
        [HttpPost]
        public IActionResult Privacy(UserUpdateViewModel model) 
        {
            var username = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool isSameName = model.Name == user.Name;
                bool isSamePassword = !string.IsNullOrWhiteSpace(model.Password) && user.Password == user.HashPassword(model.Password);

                if (!isSameName)
                {
                    user.Name = model.Name; 
                }
                

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    if (model.Password != model.PasswordAgain)
                    {
                        ModelState.AddModelError("PasswordAgain", "Şifreler eşleşmiyor.");
                        return View(model);
                    }

                    string salt = WeatherForecastApp.Models.User.GenerateSalt();
                    user.Salt = salt;
                    user.Password = user.HashPassword(model.Password);
                }

                //user.DefaultCityName = model.DefaultCityName;

                _context.SaveChanges();
                if (!isSameName || !isSamePassword)
                {
                    TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Bilgileriniz aynı, güncellenemedi.";
                }


                //TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi!";

                return RedirectToAction("Privacy");
            }

            model.Name = user.Name;
            //model.DefaultCityName = user.DefaultCityName;
            Console.WriteLine("Buraya girdi");
            return View(model); 
        }

       


    }
}
