public class PubDate
{
    /// <summary>取得某月的第一天</summary>
    public static DateTime FirstDayOfMonth(DateTime datetime)
    {
        return datetime.AddDays(1 - datetime.Day);
    }
    //// <summary>取得某月的最后一天</summary>
    public static DateTime LastDayOfMonth(DateTime datetime)
    {
        return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
    }
    //// <summary>取得上个月的最后一天</summary>
    public static DateTime LastDayOfPreviousMonth(DateTime datetime)
    {
        return datetime.AddDays(1 - datetime.Day).AddDays(-1);
    }
    //// <summary>取得下个月的第一天</summary>
    public static DateTime FirstDayOfNextMonth(DateTime datetime)
    {
        return datetime.AddDays(1 - datetime.Day).AddMonths(1);
    }
}

