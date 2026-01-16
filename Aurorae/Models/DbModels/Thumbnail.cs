namespace Aurorae.Models.DbModels;

public class Thumbnail
{
    public long Id { get; set; }

    public string FilePath { get; set; } = null!;

    public byte[] Data { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public int Width { get; set; }
    public int Height { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
