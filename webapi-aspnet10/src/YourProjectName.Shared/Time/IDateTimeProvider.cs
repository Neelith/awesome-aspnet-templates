namespace YourProjectName.Shared.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
