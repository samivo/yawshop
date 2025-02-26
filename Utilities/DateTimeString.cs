
namespace YawShop.Utilities;

public class DateTimeString
{
    /// <summary>
    /// Converts UTC datetime to some spesific timezone and returns it formatted in format dd.MM.yyyy HH:mm
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string GetHumanReadableDateTimeString(DateTime dateTime)
    {
        try
        {

            //Since all database datetime's are handled as utc, conversion is needed for example emails.
            // TODO If the timezone is stored in the database, dependency on the system timezone is avoided.
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Helsinki")).ToString("dd.MM.yyyy HH:mm");
        }
        catch (System.Exception)
        {
            throw;
        }

    }

}
