using Aurorae.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Aurorae.Controllers;

public class HomeController : Controller
{
    public IActionResult Index([FromQuery] int images = 100)
    {
        ViewBag.Images = images;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromQuery] int code = 500)
    {
        return View(new ErrorViewModel { StatusCode = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
