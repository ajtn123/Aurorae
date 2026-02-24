using Microsoft.EntityFrameworkCore;

namespace Aurorae.Services;

public class TokenAuthMiddleware(AuroraeDb db) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("access_token", out var token) ||
            !await db.AccessTokens.AnyAsync(x => x.Token == token))
        {
            context.Abort();
            return;
        }

        context.Items["Authenticated"] = true;

        await next(context);
    }
}
