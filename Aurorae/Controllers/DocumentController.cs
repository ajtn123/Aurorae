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

    [HttpPost("/doc/upload")]
    public IActionResult Upload([FromQuery] string path, List<IFormFile> files)
    {
        var targetDir = Path.GetFullPath(Path.Combine(LocalPath.Document, path));
        if (!targetDir.StartsWith(Path.GetFullPath(LocalPath.Document)))
            return Forbid();

        if (!Directory.Exists(targetDir))
            return NotFound();

        foreach (var file in files)
        {
            if (file.Length == 0) continue;
            var filePath = Path.Combine(targetDir, Path.GetFileName(file.FileName));
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
        }

        return Redirect($"/doc/{path}");
    }

    [HttpPost("/doc/create-folder")]
    public IActionResult CreateFolder([FromQuery] string path, [FromForm] string name)
    {
        var targetDir = Path.GetFullPath(Path.Combine(LocalPath.Document, path));
        if (!targetDir.StartsWith(Path.GetFullPath(LocalPath.Document)))
            return Forbid();

        if (!Directory.Exists(targetDir))
            return NotFound();

        var newDir = Path.Combine(targetDir, name);
        Directory.CreateDirectory(newDir);

        return Redirect($"/doc/{path}");
    }
}
