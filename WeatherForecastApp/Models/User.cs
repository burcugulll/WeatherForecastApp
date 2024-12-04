using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace WeatherForecastApp.Models

{
   
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Salt { get; set; }

        public int LoginAttempts { get; set; }

        public DateTime? LockoutEndTime { get; set; }

        [Required]
        public string Name { get; set; }

        
        public string UserType { get; set; }


        public string DefaultCityName {  get; set; }

        public string Status {  get; set; }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
                return Convert.ToBase64String(bytes);
            }
        }

        public static string GenerateSalt()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
        }

    }
}
