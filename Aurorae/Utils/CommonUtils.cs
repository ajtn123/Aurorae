using Microsoft.AspNetCore.WebUtilities;
using System.Reflection;

namespace Aurorae.Utils;

public static class CommonUtils
{
    public static string GetStatusString(int code) => $"{code} {ReasonPhrases.GetReasonPhrase(code)}";
    public static string GetVersionString() => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}
