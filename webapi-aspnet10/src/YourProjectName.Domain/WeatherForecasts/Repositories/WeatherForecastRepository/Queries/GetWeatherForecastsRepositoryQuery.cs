using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;
public class GetWeatherForecastsRepositoryQuery
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
}
