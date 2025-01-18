namespace YawShop.Utilities;

public class GiftcardCodeGenerator
{
    // <summary>
    ///<para> Generates giftcard code in format XXX-XXX-XXX </para>
    ///<para>Possible characters are ABCDEFGHJKLMNPQRSTUVWXYZ23456789</para>
    /// </summary>
    /// <returns>Giftcard code as string</returns>
    /// <exception cref="Exception"/>
    /// Chars like O,0,I,1 are excluded to make the code easier to read
    public static string CreateCode()
    {


        string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        char[] codeArray = new char[11];

        try
        {
            var random = new Random();

            for (int i = 0; i < 11; i++)
            {
                if (i == 3 || i == 7)
                {
                    codeArray[i] = '-';
                }
                else
                {
                    codeArray[i] = allowedChars[random.Next(0, allowedChars.Length)];
                }
            }

        }
        catch (Exception)
        {
            throw;
        }

        return new string(codeArray);

    }
}