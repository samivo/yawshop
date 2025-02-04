﻿using MimeKit;

namespace YawShop.Utilities
{
    /// <summary>
    /// giftcard template clss
    /// </summary>
    public class GiftcardEmail
    {
        /// <summary>
        /// Creates generig giftcard email in html type.
        /// </summary>
        /// <param name="GiftcardCode"></param>
        /// <returns></returns>
        public static TextPart GetEmailBody(string GiftcardCode)
        {
            var message = new TextPart("html")
            {
                Text = $@"
<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Lahjakortti</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f6f6f6;
            color: #333333;
             width: 100%;
        }}

        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            box-sizing: border-box;
        }}

        h1, h2, h3 {{
            color: #333333;
            margin: 0 0 10px;
            padding: 0;
        }}

        p {{
            margin: 0 0 15px;
            line-height: 1.5;
        }}

        a {{
            color: #1a82e2;
            text-decoration: none;
        }}

        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #777777;
            text-align: center;
        }}
    </style>
</head>

<body>
    <div class=""container"">
        <h3>Lahjakortti</h3>
        <br>
        <p>Hei! Tässä linkki lahjakorttiisi.</p>
        <p>Voit lähettää linkin lahjakortin saajalle. Älä kuitenkaan jaa linkkiä julkisesti.</p>
        <a href='https://kauppa.klu.fi/giftcard/{GiftcardCode}' > https://kauppa.klu.fi/giftcard/{GiftcardCode}</a>
        <p>Lahjakortin koodi syötetään ajanvarauksen yhteydessä. Ajan voi varata osoitteesta klu.fi</p>
        <br>
        <p>Terveisin<br>Kuopion laskuvarjourheilijat Ry<br>www.klu.fi</p>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Kuopion laskuvarjourheilijat Ry. Kaikki oikeudet pidätetään.</p>
        </div>
    </div>
</body>

</html>"
            };

            return message;
        }
    }
}
