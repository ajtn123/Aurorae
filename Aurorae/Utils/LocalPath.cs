namespace Aurorae.Utils;

public static class LocalPath
{
    public static string Gallery { get; } = Environment.GetEnvironmentVariable("AURORAE_GALLERY") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    public static string Document { get; } = Environment.GetEnvironmentVariable("AURORAE_DOCUMENT") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static string HttpProxy { get; } = Environment.GetEnvironmentVariable("AURORAE_HTTP_PROXY")!;
}