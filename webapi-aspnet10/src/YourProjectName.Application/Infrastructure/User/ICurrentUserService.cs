using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourProjectName.Application.Infrastructure.User;
public interface ICurrentUserService
{
    bool IsCurrentUserAuthenticated();
    string GetCurrentUserId();
}
