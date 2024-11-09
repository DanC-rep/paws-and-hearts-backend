using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using PawsAndHearts.Core.Models;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Framework.Authorization;

public class ClaimsManager
{
    public Result<Guid, ErrorList> GetCurrentUserId(HttpContext httpContext)
    {
        var userIdString = httpContext.User.Claims.FirstOrDefault(u => u.Type == CustomClaims.Id)?.Value;

        if (userIdString is null)
            return Errors.General.NotFound(null, "user id").ToErrorList();

        if (!Guid.TryParse(userIdString, out var userId))
            return Error.Failure("parse.error", "can not convert user id to guid").ToErrorList();

        return userId;
    }
}