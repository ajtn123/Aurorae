using Microsoft.AspNetCore.Mvc;

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
        if (!file.Exists)
            return NotFound();
        else
        {
            var etag = $"{file.LastWriteTimeUtc.ToBinary()}-{file.Length}";

            if (Request.Headers.IfNoneMatch.Contains(etag))
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            Response.Headers.ETag = etag;

            return File(file.OpenRead(), GetContentType(file.Name));
        }
    }

    public static string GetContentType(string fileName) => MimeMapping.MimeUtility.GetMimeMapping(fileName);
}
