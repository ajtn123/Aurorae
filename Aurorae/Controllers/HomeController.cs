using Aurorae.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace Aurorae.Controllers;

public class HomeController : Controller
{
    public IActionResult Index([FromQuery] int images = 100)
    {
        ViewBag.Images = images;
        return View();
    }

    [HttpGet("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromQuery] int code = 500)
    {
        return View(new ErrorViewModel { StatusCode = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("/jump.php")]
    public IActionResult Jump()
    {
        var qs = HttpContext.Request.QueryString.Value;
        if (string.IsNullOrWhiteSpace(qs) || qs.Length <= 1)
            return BadRequest();

        var url = WebUtility.UrlDecode(qs[1..]);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return BadRequest("无效链接");

        return Redirect(uri.ToString());
    }
}
