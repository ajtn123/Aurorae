using System.Diagnostics;

namespace Aurorae.Services;

public static class FastfetchAdapter
{
    private static readonly ProcessStartInfo processStartInfo = new()
    {
        FileName = "fastfetch",
        Arguments = "--logo none",
        RedirectStandardOutput = true,
        // RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    private static (DateTime Time, string Output) last = (DateTime.MinValue, "Not Initialize Yet");
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public static async Task<string> GetFastfetchOutput()
    {
        if (DateTime.Now - last.Time < CacheDuration)
            return last.Output;

        using var process = Process.Start(processStartInfo);
        if (process == null)
            return last.Output;

        var stdout = await process.StandardOutput.ReadToEndAsync();
        stdout = stdout[stdout.IndexOf('@')..(stdout.IndexOf('\u001b') - 2)];
        last = (DateTime.Now, stdout);

        return last.Output;
    }
}
