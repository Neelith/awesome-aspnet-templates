namespace YourProjectName.Core.Services.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
