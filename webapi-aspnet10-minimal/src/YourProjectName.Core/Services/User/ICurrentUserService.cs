namespace YourProjectName.Core.Services.User;

public interface ICurrentUserService
{
    bool IsCurrentUserAuthenticated();
    string GetCurrentUserId();
}
