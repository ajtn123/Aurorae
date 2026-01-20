using System.Diagnostics;

namespace Aurorae.Services;

public class FFProbeAdapter
{
    private static ProcessStartInfo GetProcessStartInfo(string path) => new()
    {
        FileName = "ffprobe",
        Arguments = $"-hide_banner \"{path}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    public async Task<string> Analyze(FileInfo file)
    {
        if (!file.Exists)
            throw new FileNotFoundException();

        var info = GetProcessStartInfo(file.FullName);
        using var process = Process.Start(info) ??
            throw new InvalidOperationException();

        var (stdout, stderr) = await Task.WhenAll(
            process.StandardOutput.ReadToEndAsync(),
            process.StandardError.ReadToEndAsync()
        ).ContinueWith(t => (t.Result[0], t.Result[1]));

        return stdout + stderr;
    }
}
