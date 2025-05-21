namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Utilities;

public static class DateTimeHelper
{
    public static DateTime ToWIB(this DateTime dateTime)
    {
        return dateTime.AddHours(7);
    }
}