namespace YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

public class GetWeatherForecastsRepositoryQuery
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
