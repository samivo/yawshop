using System.Globalization;
using System.Text.Json;
using YawShop.Services.PaytrailService.Models;
using YawShop.Utilities;

namespace YawShop.Services.PaytrailService;

public class Paytrail
{
    public static async Task<ResponseModel> CreatePaymentAsync(PaytrailRequestModel requestModel, PaytrailSettings settings)
    {


        if (string.IsNullOrEmpty(settings.Secret))
        {
            throw new NullReferenceException("Missing paytrail secret.");
        }
        if (string.IsNullOrEmpty(settings.Account))
        {
            throw new NullReferenceException("Missing paytrail account.");
        }

        var timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        var headers = new Dictionary<string, string>
        {
            { "checkout-account", settings.Account },
            { "checkout-algorithm", "sha512" },
            { "checkout-method", "POST" },
            { "checkout-nonce", PaytrailCrypto.RandomDigits(20) },
            { "checkout-timestamp", timestamp }
        };


        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var body = JsonSerializer.Serialize(requestModel, serializeOptions);

        var encData = PaytrailCrypto.CalculateHmac(settings.Secret, headers, body, headers["checkout-algorithm"]);

        var client = new HttpClient();
        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://services.paytrail.com/payments"),
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
            Headers = {
                { "checkout-account", headers["checkout-account"] },
                { "checkout-algorithm", headers["checkout-algorithm"] },
                { "checkout-method", headers["checkout-method"] },
                { "checkout-nonce", headers["checkout-nonce"] },
                { "checkout-timestamp", headers["checkout-timestamp"] },
                { "signature", encData }
            }
        };

        var response = await client.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Paytrail new payment request failed. Http response statuscode: {response.StatusCode}. Message: {await response.Content.ReadAsStringAsync()}");
        }

        var responseHeaders = new Dictionary<string, string>();

        foreach (var header in response.Headers)
        {
            var value = header.Value.FirstOrDefault();

            if (value != null)
            {
                responseHeaders.Add(header.Key, value);
            }
            else
            {
                throw new InvalidOperationException("Paytrail create new payment response: signature header is null.");
            }

        }

        var calculatedResponseSignature = PaytrailCrypto.CalculateHmac(settings.Secret, responseHeaders, await response.Content.ReadAsStringAsync(), headers["checkout-algorithm"]);

        foreach (var header in response.Headers)
        {
            if (header.Key == "signature")
            {

                if (header.Value != null)
                {

                    if (header.Value.FirstOrDefault() != calculatedResponseSignature)
                    {
                        throw new InvalidOperationException("Signature mismatch! Received signature from paytrail create payment response header wont match with calculated signature!");
                    }
                }
            }
        }

        //Deserialize paytrail's response to object
        var resonseString = await response.Content.ReadAsStringAsync();
        var responseModel = JsonSerializer.Deserialize<ResponseModel>(resonseString, serializeOptions) ?? throw new InvalidOperationException("Paytrail response object is null");


        return responseModel;

    }
}