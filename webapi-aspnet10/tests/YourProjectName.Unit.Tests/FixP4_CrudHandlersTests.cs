using Moq;
using YourProjectName.Application.Features.WeatherForecasts.DeleteWeatherForecastById;
using YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;
using YourProjectName.Application.Infrastructure.Persistence;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

// NOTE: This file references handler and command types that DO NOT EXIST YET
// in the classic template (P4 — Full CRUD).
// Expected RED: COMPILE FAILURE.
// The implementer must create:
//   - UpdateWeatherForecastByIdCommandHandler
//   - UpdateWeatherForecastByIdCommand
//   - DeleteWeatherForecastByIdCommandHandler
//   - DeleteWeatherForecastByIdCommand
//   - WeatherForecast.Update(...) rich-domain method
//   - IWeatherForecastRepository.GetById(...)
// Once created, these tests will compile and run, then fail at runtime until
// the handler logic is implemented.

namespace YourProjectName.Unit.Tests;

public sealed class FixP4_CrudHandlersTests
{
    // ============================================================
    // P4 Update — compile-time RED: types don't exist yet
    // ============================================================

    [Fact]
    public async Task UpdateWeatherForecastByIdCommandHandler_ValidUpdate_CallsRepositoryAndSaves()
    {
        // ARRANGE
        var existingForecast = WeatherForecast.Create(
            new DateOnly(2025, 6, 1), 25, "Warm").Value!;

        var repoMock = new Mock<IWeatherForecastRepository>();
        repoMock.Setup(r => r.GetById(existingForecast.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingForecast);

        var uowMock = new Mock<IUnitOfWork>();

        // COMPILE ERROR: type 'UpdateWeatherForecastByIdCommandHandler' does not exist
        var handler = new UpdateWeatherForecastByIdCommandHandler(repoMock.Object, uowMock.Object);

        // COMPILE ERROR: type 'UpdateWeatherForecastByIdCommand' does not exist
        var command = new UpdateWeatherForecastByIdCommand
        {
            Id = existingForecast.Id,
            Date = new DateOnly(2025, 7, 1),
            TemperatureC = 30,
            Summary = "Hot"
        };

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsSuccess);
        repoMock.Verify(r => r.GetById(existingForecast.Id, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWeatherForecastByIdCommandHandler_NotFound_ReturnsNotFound()
    {
        // ARRANGE
        var repoMock = new Mock<IWeatherForecastRepository>();
        repoMock.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecast?)null);

        var uowMock = new Mock<IUnitOfWork>();

        // COMPILE ERROR: type 'UpdateWeatherForecastByIdCommandHandler' does not exist
        var handler = new UpdateWeatherForecastByIdCommandHandler(repoMock.Object, uowMock.Object);

        var command = new UpdateWeatherForecastByIdCommand
        {
            Id = 999,
            TemperatureC = 30
        };

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Contains(result.Metadata, m =>
            m.Key == "errorType" && m.Value == "404");
        repoMock.Verify(r => r.GetById(999, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ============================================================
    // P4 Delete — compile-time RED: types don't exist yet
    // ============================================================

    [Fact]
    public async Task DeleteWeatherForecastByIdCommandHandler_ExistingRecord_MarksAsDeletedAndSaves()
    {
        // ARRANGE
        var existingForecast = WeatherForecast.Create(
            new DateOnly(2025, 6, 1), 25, "Warm").Value!;

        var repoMock = new Mock<IWeatherForecastRepository>();
        repoMock.Setup(r => r.GetById(existingForecast.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingForecast);

        var uowMock = new Mock<IUnitOfWork>();

        // COMPILE ERROR: type 'DeleteWeatherForecastByIdCommandHandler' does not exist
        var handler = new DeleteWeatherForecastByIdCommandHandler(repoMock.Object, uowMock.Object);

        // COMPILE ERROR: type 'DeleteWeatherForecastByIdCommand' does not exist
        var command = new DeleteWeatherForecastByIdCommand { Id = existingForecast.Id };

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsSuccess);
        Assert.True(existingForecast.Deleted);
        repoMock.Verify(r => r.GetById(existingForecast.Id, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteWeatherForecastByIdCommandHandler_NotFound_ReturnsNotFound()
    {
        // ARRANGE
        var repoMock = new Mock<IWeatherForecastRepository>();
        repoMock.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecast?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var handler = new DeleteWeatherForecastByIdCommandHandler(repoMock.Object, uowMock.Object);

        var command = new DeleteWeatherForecastByIdCommand { Id = 999 };

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Contains(result.Metadata, m =>
            m.Key == "errorType" && m.Value == "404");
        repoMock.Verify(r => r.GetById(999, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
