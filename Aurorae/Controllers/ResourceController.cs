using Microsoft.AspNetCore.Mvc;
using Aurorae.Models;
using Aurorae.Utils;
using System.Net.Mime;

namespace Aurorae.Controllers;

public class ResourceController : Controller
{
    [HttpGet("/resources/images/{*name}")]
    public IActionResult GetImage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return NotFound();

        var path = Path.Combine(LocalPath.Gallery, name);
        var file = new FileInfo(path);
        if (file.Exists)
            return File(file.OpenRead(), GetContentType(file.Name));
        else
            return NotFound();
    }

    public static string GetContentType(string fileName) => MimeMapping.MimeUtility.GetMimeMapping(fileName);
}
