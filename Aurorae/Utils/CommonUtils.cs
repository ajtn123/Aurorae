using Microsoft.AspNetCore.WebUtilities;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Aurorae.Utils;

public static partial class CommonUtils
{
    public static string GetStatusString(int code) => $"{code} {ReasonPhrases.GetReasonPhrase(code)}";
    public static string GetVersionString() => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

    [GeneratedRegex("^URL=(?<url>.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex IniUrlProp();
    public static string? GetLink(FileInfo file)
    {
        if (!file.Exists)
            return null;

        foreach (var line in File.ReadLines(file.FullName))
            if (IniUrlProp().IsMatch(line))
                return IniUrlProp().Match(line).Groups["url"].Value;

        return null;
    }
}
