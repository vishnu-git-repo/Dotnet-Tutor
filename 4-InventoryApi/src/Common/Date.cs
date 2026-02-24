namespace App.Common;

public static class DateHelper
{
    public static int CalculateDuration(DateTime start, DateTime end)
    {
        var duration = end - start;
        return duration.Days;
    }
}