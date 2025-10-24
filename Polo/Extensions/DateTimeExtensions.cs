namespace Polo.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime object to a Unix timestamp (seconds since Unix epoch).
    /// </summary>
    /// <param name="dateTime">The DateTime object to convert.</param>
    /// <returns>An integer representing the Unix timestamp.</returns>
    public static int ToUnixTime(this DateTime dateTime)
    {
        // Define the Unix epoch start date (January 1, 1970, 00:00:00 UTC)
        DateTime unixEpoch = DateTime.UnixEpoch;

        // Calculate the difference in seconds.
        // Ensure the input dateTime is in UTC for accurate calculation against the UTC epoch.
        TimeSpan diff = dateTime.ToUniversalTime() - unixEpoch;

        // Return the total seconds as an integer.
        // Note: This will truncate any milliseconds.
        // If the number of seconds exceeds the maximum value of an int, it will overflow.
        // For very large time differences, consider using 'long' if 'int' is not sufficient.
        return (int)diff.TotalSeconds;
    }
}
