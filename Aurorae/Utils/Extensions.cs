namespace Aurorae.Utils;

public static class Extensions
{
    extension(int)
    {
        public static int? TryParse(object? obj) => int.TryParse(obj?.ToString(), out var v) ? v : null;
    }

    extension(DateTimeOffset time)
    {
        public string ToLocalString(string format = "g") => time.ToLocalTime().ToString(format);
    }
}
