using Microsoft.EntityFrameworkCore;

namespace Aurorae.Services;

public class TokenAuthMiddleware(AuroraeDb db) : IMiddleware
{
    private static HashSet<string> tokens = [];
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("access_token", out var token))
        {
            context.Abort();
            return;
        }

        static bool IsValid(string token) => tokens.Contains(token);

        if (!IsValid(token))
        {
            tokens = await db.AccessTokens.Select(x => x.Token).ToHashSetAsync();
        }

        if (!IsValid(token))
        {
            context.Abort();
            return;
        }

        context.Items["Authenticated"] = true;

        await next(context);
    }
}
