using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Infrastructure.Persistence.Configurations
{
    public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
    {
        public void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.ToTable("Forecasts").HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.OwnsOne(c => c.Summary, (summaryBuilder) =>
            {
                summaryBuilder.Property(c => c.Value)
                    .HasColumnName("Summary")
                    .HasMaxLength(256);
            });
        }
    }
}
