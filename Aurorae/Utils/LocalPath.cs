namespace Aurorae.Utils;

public static class LocalPath
{
    public static string Gallery => Environment.GetEnvironmentVariable("AuroraeGallery") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
}