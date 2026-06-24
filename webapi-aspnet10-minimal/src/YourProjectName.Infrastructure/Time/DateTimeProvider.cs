using YourProjectName.Core.Services.Time;

namespace YourProjectName.Infrastructure.Time;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow.ToUniversalTime();
}
