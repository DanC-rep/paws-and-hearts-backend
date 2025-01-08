using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Contracts;
using PawsAndHearts.Core.Models;

namespace PawsAndHearts.Framework.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionRequirementHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionAttribute permission)
    {
        var userScopedData = _httpContextAccessor.HttpContext?.RequestServices
            .GetRequiredService<UserScopedData>();

        if (userScopedData != null && userScopedData.Permissions.Contains(permission.Code))
            context.Succeed(permission);

        return Task.CompletedTask;
    }
}