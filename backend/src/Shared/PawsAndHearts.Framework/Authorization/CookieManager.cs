using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Framework.Authorization;

public static class CookieManager
{
    public static Result<Guid, ErrorList> GetRefreshToken(HttpContext httpContext)
    {
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Error.Null("refresh.token", "refresh token is null").ToErrorList();

        return Guid.Parse(refreshToken);
    }

    public static void AddRefreshTokenToCookies(HttpContext httpContext, Guid refreshToken)
    {
        httpContext.Response.Cookies.Append("refreshToken", refreshToken.ToString());
    }
}