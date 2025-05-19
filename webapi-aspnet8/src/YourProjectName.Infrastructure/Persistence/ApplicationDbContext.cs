using System.Reflection;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Application.Infrastructure.Persistance;
using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        //Set your DB sets here, put them in the IApplicationDbContext interface first
        public DbSet<WeatherForecast> Forecasts { get; set; }
    }
}
