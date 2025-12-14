using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
public class CreateWeatherForecastRepositoryCommand
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
