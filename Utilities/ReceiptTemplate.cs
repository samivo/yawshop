using System.Text;
using MimeKit;
using YawShop.Services.CheckoutService.Models;
using YawShop.Services.ClientService.Models;

namespace YawShop.Utilities
{
    /// <summary>
    /// giftcard template clss
    /// </summary>
    public class ReceiptTemplate
    {
        /// <summary>
        /// Creates generig giftcard email in html type.
        /// </summary>
        /// <param name="GiftcardCode"></param>
        /// <returns></returns>
        public static TextPart GetEmailBody(CheckoutModel checkout)
        {
            var total = 0;
            var productListHtml = new StringBuilder();

            productListHtml.Append("<ul>");
            foreach (var product in checkout.Products)
            {
                total += product.UnitPrice;
                productListHtml.Append(

                    $"<li>{product.ProductName} {product.UnitPrice / 100} € {product.Units} kpl.</li>"
                );
            }
            productListHtml.Append("</ul>");

            var message = new TextPart("html")
            {
                Text = $@"
            <!DOCTYPE html>
            <html lang=""fi"">

            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Kuitti</title>
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
                    <h3>Kiitos asioinnista {checkout.Client.FirstName}.</h3>
                    <br>
                    {productListHtml}
                    <p>Yhteensä {total / 100} €</p
                    <br>
                    <p>Tapahtuma viite: {checkout.Reference}</p>
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
