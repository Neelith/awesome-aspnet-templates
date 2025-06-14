﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using YourProjectName.Application.Infrastructure.User;

namespace YourProjectName.Infrastructure.User;
internal class CurrentUserService(
    ILogger<CurrentUserService> logger, 
    IHttpContextAccessor httpContextAccessor) 
    : ICurrentUserService
{
    public string GetCurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user is not { Identity.IsAuthenticated: true })
        {
            logger.LogError("Attempted to get current user ID, but user is not authenticated.");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        string? userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogError("Current user ID is null or empty.");
            throw new InvalidOperationException("Current user ID is null or empty.");
        }

        logger.LogDebug("Current user ID retrieved: {UserId}", userId);

        return userId;
    }

    public bool IsCurrentUserAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
