using Aurorae.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;

namespace Aurorae.Utils;

public static class ActionUtils
{
    public static IActionResult IfNoneMatch(this ControllerBase controller, FileInfo file)
        => controller.IfNoneMatch(file, file => controller.File(file.OpenRead(), MimeUtility.GetMimeMapping(file.Name)));

    public static IActionResult IfNoneMatch<TContent>(this ControllerBase controller, TContent data, Func<TContent, IActionResult> result)
    {
        var etag = GetETag(data);

        if (controller.Request.Headers.IfNoneMatch.Contains(etag))
            return controller.StatusCode(StatusCodes.Status304NotModified);

        controller.Response.Headers.ETag = etag;

        return result(data);
    }

    public static string GetETag<TContent>(TContent data) => data switch
    {
        string etag => etag,
        FileInfo file => $"{file.LastWriteTimeUtc.Ticks}-{file.Length}",
        Thumbnail thumbnail => $"{thumbnail.CreatedAt.UtcTicks}-{thumbnail.Data.Length}-{thumbnail.Width}-{thumbnail.Height}",
        _ => throw new NotImplementedException()
    };
}
