using Aurorae.Models.Gallery;
using Microsoft.AspNetCore.Mvc;

namespace Aurorae.Controllers;

public class DocumentController : Controller
{
    [HttpGet("/doc/{*name}")]
    public IActionResult GetItem([FromRoute] string name, [FromQuery] string? filter = null, [FromQuery] bool recursive = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return View("Folder", new FolderViewModel(LocalPath.Document, filter, recursive));

        var path = Path.Combine(LocalPath.Document, name);

        if (Directory.Exists(path))
            return View("Folder", new FolderViewModel(path, filter, recursive));

        else if (new FileInfo(path) is { Exists: true } file)
            return this.IfNoneMatch(file);

        return NotFound();
    }
}
