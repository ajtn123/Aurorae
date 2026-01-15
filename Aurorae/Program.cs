global using Aurorae.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AuroraeDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AuroraeDBConnection")));
builder.Services.AddScoped<TokenAuthMiddleware>();

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(10000);
        options.ListenAnyIP(8443, listen =>
        {
            listen.UseHttps(https =>
            {
                https.ServerCertificate = X509Certificate2.CreateFromPemFile("/etc/ssl/cloudflare/cert.pem", "/etc/ssl/cloudflare/key.pem");
            });
        });
    });

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear(); // Cloudflare IPs are many
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders();
}

app.UseMiddleware<TokenAuthMiddleware>();

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
