global using Aurorae.Services;
global using Aurorae.Utils;
using Aurorae.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NReco.Logging.File;
using Scighost.PixivApi;
using System.Globalization;

var culture = new CultureInfo("zh-CN");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
_ = FastfetchAdapter.GetFastfetchOutput();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AuroraeDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AuroraeDBConnection")));
builder.Services.AddSingleton<IThumbnailGenerator, AvifThumbnailGenerator>();
builder.Services.AddSingleton<ImageSourceProvider>();
builder.Services.AddSingleton<PixivClient>(services => new(httpProxy: LocalPath.HttpProxy));
builder.Services.AddScoped<PixivAdapter>();
builder.Services.AddSingleton<FFProbeAdapter>();

if (builder.Environment.IsProduction())
{
    builder.Services.AddScoped<FileSyncService>();
    builder.Services.AddHostedService<FileSyncWorker>();
    builder.Services.AddScoped<TokenAuthMiddleware>();
    builder.Services.AddRequestDecompression();
    builder.Services.AddResponseCompression(options => options.EnableForHttps = true);
    builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
        options.Preload = true;
        options.IncludeSubDomains = false;
    });

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
    app.UseStatusCodePagesWithReExecute("/error", "?code={0}");
    app.MapGet("/log", (IConfiguration config) => File.ReadAllTextAsync(config["Logging:File:Path"]!));
    app.MapGet("/version", () => CommonUtils.GetVersionString());

    app.UseForwardedHeaders();
    app.UseHsts();
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
