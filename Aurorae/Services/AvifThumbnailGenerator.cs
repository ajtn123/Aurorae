using Aurorae.Interfaces;
using FFMpegCore;
using FFMpegCore.Enums;

namespace Aurorae.Services;

public class AvifThumbnailGenerator() : IThumbnailGenerator
{
    public async Task<byte[]> GenerateAsync(string filePath, int width, int height)
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
        finally
        {
            File.Delete(temp);
        }
    }

    public string ContentType { get; } = "image/avif";
}
