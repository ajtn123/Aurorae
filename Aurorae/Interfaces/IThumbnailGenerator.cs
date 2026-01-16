namespace Aurorae.Interfaces;

public interface IThumbnailGenerator
{
    Task<byte[]> GenerateAsync(string filePath, int width, int height);
    string ContentType { get; }
}
