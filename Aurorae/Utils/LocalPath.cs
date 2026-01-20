namespace Aurorae.Utils;

public static class LocalPath
{
    public static string Gallery { get; } = Environment.GetEnvironmentVariable("AURORAE_GALLERY") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    public static string HttpProxy { get; } = Environment.GetEnvironmentVariable("AURORAE_HTTP_PROXY")!;
}