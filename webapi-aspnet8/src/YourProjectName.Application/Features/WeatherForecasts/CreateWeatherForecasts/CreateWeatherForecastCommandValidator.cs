﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;
public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
{
    public CreateWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100).WithMessage("Temperature must be between -100 and 100.");
        RuleFor(x => x.Summary).MaximumLength(50).WithMessage("Summary cannot exceed 50 characters.");
    }
}
