using Aurorae.Models.Gallery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Controllers;

public class GalleryController(AuroraeDb db) : Controller
{
    [HttpGet("/gallery/{*name}")]
    public IActionResult GetItem([FromRoute] string name, [FromQuery] bool recursive = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return View("Folder", new FolderViewModel(LocalPath.Gallery, recursive));

        var path = Path.Combine(LocalPath.Gallery, name);
        if (Directory.Exists(path))
            return View("Folder", new FolderViewModel(path, recursive));
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
}
