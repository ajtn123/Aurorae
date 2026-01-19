using System.Text.RegularExpressions;

namespace Aurorae.Services;

public class ImageSourceProvider
{
    private readonly IImageSource[] sources = [new PixivImageSource()];
    public dynamic? GetSource(FileInfo file)
        => sources.Select(x => x.GetSource(file)).FirstOrDefault(x => x != null, null);
}

public interface IImageSource
{
    dynamic? GetSource(FileInfo file);
}

public partial class PixivImageSource : IImageSource
{
    [GeneratedRegex(@"^\[(?<pid>[0-9]+)\](?<index>[0-9]*) ?(?<title>.*).[A-Za-z]+$")]
    private static partial Regex Pattern();

    public string Name => "Pixiv";
    public dynamic? GetSource(FileInfo file)
    {
        if (!Pattern().IsMatch(file.Name))
            return null;

        var info = Pattern().Match(file.Name).Groups;
        var pid = int.TryParse(info["pid"]);
        var index = (int.TryParse(info["index"]) ?? 0) + 1;

        var url = $"https://www.pixiv.net/artworks/{pid}#{index}";

        return new { Name, Url = url, Pid = pid, Index = index };
    }
}