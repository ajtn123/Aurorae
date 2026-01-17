using Aurorae.Models.Gallery;
using Microsoft.AspNetCore.Mvc;

namespace Aurorae.Controllers;

public class GalleryController : Controller
{
    [HttpGet("/gallery/{*name}")]
    public IActionResult Item([FromRoute] string name, [FromQuery] bool recursive = false)
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
}
