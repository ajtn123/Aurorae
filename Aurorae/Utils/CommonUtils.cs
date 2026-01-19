using Microsoft.AspNetCore.WebUtilities;

namespace Aurorae.Utils;

public static class CommonUtils
{
    public static string GetStatusString(int code) => $"{code} {ReasonPhrases.GetReasonPhrase(code)}";
}
