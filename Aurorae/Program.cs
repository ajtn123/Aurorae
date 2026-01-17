global using Aurorae.Services;
global using Aurorae.Utils;
using Aurorae.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

var culture = new CultureInfo("zh-CN");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AuroraeDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AuroraeDBConnection")));
builder.Services.AddSingleton<IThumbnailGenerator, AvifThumbnailGenerator>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<TokenAuthMiddleware>();
    builder.Services.AddRequestDecompression();
    builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

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
        options.KnownProxies.Clear();
    });
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

    app.UseForwardedHeaders();

    app.UseMiddleware<TokenAuthMiddleware>();
    app.UseRequestDecompression();
    app.UseResponseCompression();
}

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
