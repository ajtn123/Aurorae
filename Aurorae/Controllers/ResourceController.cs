using Aurorae.Interfaces;
using Aurorae.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Controllers;

public class ResourceController : Controller
{
    [HttpGet("/resources/images/{*name}")]
    public IActionResult GetImage(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            GetContentType(name) is not { } type ||
            !type.StartsWith("image"))
            return NotFound();

        var file = new FileInfo(Path.Combine(LocalPath.Gallery, name));
        if (!file.Exists)
            return NotFound();

        return this.IfNoneMatch(file);
    }

    [HttpGet("/resources/text/{*name}")]
    public IActionResult GetText(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            GetContentType(name) is not { } type ||
            !type.StartsWith("text"))
            return NotFound();

        var file = new FileInfo(Path.Combine(LocalPath.Gallery, name));
        if (!file.Exists)
            return NotFound();

        return this.IfNoneMatch(file, file => Content(System.IO.File.ReadAllText(file.FullName), type));
    }

    [HttpGet("/resources/thumbnails/{*name}")]
    public async Task<IActionResult> GetThumbnail(
        [FromRoute] string name,
        [FromServices] AuroraeDb db,
        [FromServices] IThumbnailGenerator generator,
        [FromQuery] int width = -1,
        [FromQuery] int height = 480)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            GetContentType(name) is not { } type ||
            !type.StartsWith("image"))
            return NotFound();

        var file = new FileInfo(Path.Combine(LocalPath.Gallery, name));
        if (!file.Exists)
            return NotFound();
        if (file.Length <= 1 << 16)
            return GetImage(name);

        try
        {
            var thumbnail = await GenerateThumbnail(db, generator, file, name, width, height);
            return this.IfNoneMatch(thumbnail, thumbnail => File(thumbnail.Data, thumbnail.MimeType));
        }
        catch
        {
            return NotFound();
        }
    }

    private static Task<Thumbnail?> SearchThumbnail(AuroraeDb db, string name, int width, int height)
    {
        return db.Thumbnails.AsNoTracking().FirstOrDefaultAsync(t => t.FilePath == name && t.Width == width && t.Height == height);
    }

    private static readonly SemaphoreSlim thumbnailGenerationSemaphore = new(1);

    private static async Task<Thumbnail> GenerateThumbnail(AuroraeDb db, IThumbnailGenerator generator, FileInfo file, string name, int width, int height)
    {
        await thumbnailGenerationSemaphore.WaitAsync();
        try
        {
            if (await SearchThumbnail(db, name, width, height) is { } thumbnail)
                return thumbnail;

            thumbnail = new Thumbnail
            {
                FilePath = name,
                Data = await generator.GenerateAsync(file.FullName, width, height) ?? throw new Exception(),
                Width = width,
                Height = height,
                MimeType = generator.ContentType,
            };

            db.Thumbnails.Add(thumbnail);
            await db.SaveChangesAsync();

            return thumbnail;
        }
        finally
        {
            thumbnailGenerationSemaphore.Release();
        }
    }

    [HttpGet("/resources/analyses/{*name}")]
    public async Task<IActionResult> Analyze(string name, [FromServices] FFProbeAdapter probe)
    {
        if (name.EndsWith("/ffprobe.log"))
            name = name[..^12];
        if (string.IsNullOrWhiteSpace(name) ||
            GetContentType(name) is not { } type ||
            !type.StartsWith("image"))
            return NotFound();

        var file = new FileInfo(Path.Combine(LocalPath.Gallery, name));
        if (!file.Exists)
            return NotFound();

        var analysis = await probe.Analyze(file);
        return Content(analysis);
    }

    public static string GetContentType(string name) => MimeMapping.MimeUtility.GetMimeMapping(name);
}
