namespace SmartCraft.Core.Tellus.Domain.Utility;
public static class DateTimeExtension
{
    public static string ToIso8601(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
    }

    public static string ToIso8601(this DateTime? dateTime)
    {
        return dateTime?.ToUniversalTime().ToString("u").Replace(" ", "T");
    }
}
