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

    private static readonly SemaphoreSlim thumbnailLock = new(1);
    [HttpGet("/resources/thumbnails/{*name}")]
    public async Task<IActionResult> GetThumbnail(
        [FromRoute] string name,
        [FromServices] AuroraeDb db,
        [FromServices] IThumbnailGenerator thumbnailGenerator,
        [FromQuery] int width = -1,
        [FromQuery] int height = 480)
    {
        if (string.IsNullOrWhiteSpace(name) || !GetContentType(name).StartsWith("image"))
            return NotFound();

        var file = new FileInfo(Path.Combine(LocalPath.Gallery, name));
        if (!file.Exists)
            return NotFound();
        if (file.Length <= 1 << 16)
            return GetImage(name);

        await thumbnailLock.WaitAsync();
        try
        {
            if (await db.Thumbnails.AsNoTracking().FirstOrDefaultAsync(t => t.FilePath == name && t.Width == width && t.Height == height) is { } thumbnail)
                return ServeThumbnail(thumbnail);

            var data = await thumbnailGenerator.GenerateAsync(file.FullName, width, height);
            thumbnail = new Thumbnail
            {
                FilePath = name,
                Data = data,
                Width = width,
                Height = height,
                MimeType = thumbnailGenerator.ContentType,
            };

            db.Thumbnails.Add(thumbnail);
            await db.SaveChangesAsync();

            return ServeThumbnail(thumbnail);
        }
        finally
        {
            thumbnailLock.Release();
        }
    }

    private IActionResult ServeThumbnail(Thumbnail thumbnail)
        => this.IfNoneMatch(thumbnail, thumbnail => File(thumbnail.Data, thumbnail.MimeType));

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
