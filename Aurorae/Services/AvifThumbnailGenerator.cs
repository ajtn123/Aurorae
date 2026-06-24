using Aurorae.Interfaces;
using FFMpegCore;
using FFMpegCore.Enums;

namespace Aurorae.Services;

public class AvifThumbnailGenerator(ILogger<AvifThumbnailGenerator> logger) : IThumbnailGenerator
{
    public async Task<byte[]?> GenerateAsync(string filePath, int width, int height)
    {
        var temp = Path.GetTempFileName() + ".avif";
        try
        {
            await FFMpegArguments
                .FromFileInput(filePath)
                .OutputToFile(temp, true, options => options
                    .WithVideoCodec(VideoCodec.LibaomAv1)
                    .WithConstantRateFactor(43)
                    .WithCustomArgument("-cpu-used 8")
                    .WithVideoFilters(filters => filters.Scale(width, height))
                    .ForcePixelFormat("yuv420p")
                    .ForceFormat("avif"))
                .ProcessAsynchronously();
            return await File.ReadAllBytesAsync(temp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GenerateAsync(file: {file}, width: {width}, height: {height})", filePath, width, height);
            return null;
        }
        finally
        {
            File.Delete(temp);
        }
    }

    public string ContentType { get; } = "image/avif";
}
