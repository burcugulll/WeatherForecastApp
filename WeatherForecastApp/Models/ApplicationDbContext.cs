using Microsoft.EntityFrameworkCore;
namespace WeatherForecastApp.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<WeatherInfo> WeatherInfos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
