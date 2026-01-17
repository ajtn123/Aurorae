using System.Text.RegularExpressions;

namespace Aurorae.Services;

public class ImageSourceProvider
{
    private readonly IImageSource[] sources = [new PixivImageSource()];
    public ImageSourceInfo? GetSource(FileInfo file)
        => sources.Where(x => x.IsMatch(file))
            .Select(x => new ImageSourceInfo(x.Name, x.GetSourceUrl(file)))
            .FirstOrDefault();
}

public record ImageSourceInfo(string Type, string Url);

public interface IImageSource
{
    string Name { get; }
    bool IsMatch(FileInfo file);
    string GetSourceUrl(FileInfo file);
}

public partial class PixivImageSource : IImageSource
{
    [GeneratedRegex(@"^\[(?<pid>[0-9]+)\](?<index>[0-9]*) ?(?<title>.*).[A-Za-z]+$")]
    private static partial Regex Pattern();

    public string Name => "Pixiv";
    public bool IsMatch(FileInfo file) => Pattern().IsMatch(file.Name);
    public string GetSourceUrl(FileInfo file)
    {
        var info = Pattern().Match(file.Name).Groups;

        var url = $"https://www.pixiv.net/artworks/{info["pid"]}";
        if (int.TryParse(info["index"].Value, out var index))
            url += $"#{index + 1}";
        return url;
    }
}