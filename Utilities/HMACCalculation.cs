using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace YawShop.Utilities;

public class PaytrailCrypto
{
    static readonly string[] supportedEnc = ["sha256", "sha512"];

    /// <summary>
    /// Calculate SHA Hash
    /// </summary>
    /// <param name="message">Raw string</param>
    /// <param name="secret">Shared secret</param>
    /// <param name="encType">encryption Type: sha256 or sha512</param>
    /// <returns>string</returns>
    private static string ComputeShaHash(string message, string secret, string encType = "sha256")
    {
        if (!supportedEnc.Any(e => e.Equals(encType, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception("Not supported encryption");
        }

        var key = Encoding.UTF8.GetBytes(secret);
        string outMsg = "";
        if (encType.Equals("sha512", StringComparison.InvariantCultureIgnoreCase))
        {
            using (var hmac = new HMACSHA512(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                outMsg = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        else
        {
            using (var hmac = new HMACSHA256(key))
            {
                // ComputeHash - returns byte array
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                outMsg = BitConverter.ToString(hash).Replace("-", "").ToLower();

            }
        }
        return outMsg;
    }

    /// <summary>
    /// Calculate HMAC
    /// </summary>
    /// <param name="secret">Shared secret</param>
    /// <param name="hparams">params Headers or query string parameters</param>
    /// <param name="body">body Request body or empty string for GET requests</param>
    /// <param name="encType">encryption Type: sha256 or sha512</param>
    /// <returns>string</returns>
    public static string CalculateHmac(string secret, Dictionary<string, string> hparams, string body = "", string encType = "sha256")
    {
        // Keep only checkout- params, more relevant for response validation.Filter query
        // string parameters the same way - the signature includes only checkout- values.
        // Keys must be sorted alphabetically
        var includedKeys = hparams.Where(h => h.Key.StartsWith("checkout-")).OrderBy(h => h.Key).ToList();
        List<string> data = new List<string>();
        foreach (var pair in includedKeys)
        {
            var row = string.Format("{0}:{1}", pair.Key, hparams[pair.Key]);
            data.Add(row);
        }
        data.Add(body);

        return ComputeShaHash(string.Join("\n", data.ToArray()), secret, encType);
    }

    /// <summary>
    /// Random Digits by length
    /// </summary>
    /// <param name="length">Length of generated number</param>
    /// <returns>string</returns>
    public static string RandomDigits(int length)
    {
        var random = new Random();
        string s = string.Empty;
        for (int i = 0; i < length; i++)
            s = string.Concat(s, random.Next(10).ToString());
        return s;
    }


}
