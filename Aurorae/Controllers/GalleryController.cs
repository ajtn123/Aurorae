using Aurorae.Models.Gallery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Controllers;

public class GalleryController(AuroraeDb db) : Controller
{
    [HttpGet("/gallery/{*name}")]
    public IActionResult GetItem([FromRoute] string name, [FromQuery] string? filter = null, [FromQuery] bool recursive = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return View("Folder", new FolderViewModel(LocalPath.Gallery, filter, recursive));

        var path = Path.Combine(LocalPath.Gallery, name);
        if (Directory.Exists(path))
            return View("Folder", new FolderViewModel(path, filter, recursive));
        else if (System.IO.File.Exists(path))
            return View("File", new FileViewModel(path));
        else
            return NotFound();
    }

    [HttpGet("/gallery/random")]
    public IActionResult GetRandomItems([FromQuery] int count = 100)
    {
        return PartialView("_CardList", RandomItems(db, count).Select(x => ("_ThumbnailCard", (object)x)));
    }

    public static IEnumerable<FileViewModel> RandomItems(AuroraeDb db, int count) => db.FileMetas
        .OrderBy(x => EF.Functions.Random())
        .AsEnumerable()
        .Select(x => new FileViewModel(Path.Combine(LocalPath.Gallery, x.FilePath)))
        .Where(x => x.IsImage)
        .Take(count);

    [HttpPost("/gallery/collect/{*name}")]
    public async Task<IActionResult> CollectItem([FromRoute] string name, [FromForm] bool collect)
    {
        if (await db.FileMetas.FirstOrDefaultAsync(t => t.FilePath == name) is { } file)
        {
            file.Favorite = collect;
            await db.SaveChangesAsync();
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet("/gallery/favorites")]
    public async Task<IActionResult> GetFavorites([FromQuery] string? filter = null)
    {
        var results = db.FileMetas
            .AsNoTracking()
            .Where(x => x.Favorite)
            .Select(x => x.FilePath)
            .AsAsyncEnumerable();

        if (!string.IsNullOrEmpty(filter))
            results = results.Where(x => x.Contains(filter, StringComparison.OrdinalIgnoreCase));

        var favorites = await results
            .Select(x => new FileViewModel(Path.Combine(LocalPath.Gallery, x)))
            .ToArrayAsync();
        return View("Favorites", favorites);
    }
}
