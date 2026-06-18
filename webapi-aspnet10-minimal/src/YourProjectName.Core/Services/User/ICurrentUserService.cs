namespace YourProjectName.Application.Infrastructure.User;

public interface ICurrentUserService
{
    bool IsCurrentUserAuthenticated();
    string GetCurrentUserId();
}
