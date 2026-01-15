using Microsoft.EntityFrameworkCore;

namespace Aurorae.Utils;

public class TokenAuthMiddleware(AuroraeDb db) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("access_token", out var token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing Access Token");
            return;
        }

        if (!await db.AccessTokens.AnyAsync(x => x.Token == token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Access Token");
            return;
        }

        context.Items["Authenticated"] = true;

        await next(context);
    }
}
