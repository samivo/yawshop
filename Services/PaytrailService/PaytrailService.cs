using Microsoft.Extensions.Options;
using YawShop.Services.CheckoutService.Models;
using YawShop.Services.PaymentService;
using YawShop.Services.PaytrailService.Models;
using YawShop.Utilities;

namespace YawShop.Services.PaytrailService;

public class PaytrailService : IPaymentService
{

    private readonly ILogger<PaytrailService> _logger;
    private readonly PaytrailSettings _paytrailSettings;

    public PaytrailService(ILogger<PaytrailService> logger, IOptions<PaytrailSettings> paytrailSettings)
    {
        _logger = logger;
        _paytrailSettings = paytrailSettings.Value;
    }

    public async Task<string> CreateAsync(CheckoutModel checkout)
    {
        try
        {
            var request = new PaytrailRequestModel
            {
                Reference = checkout.Reference,

                Customer = new PaytrailCustomerModel
                {
                    Email = checkout.Client.Email,
                    FirstName = checkout.Client.FirstName,
                    LastName = checkout.Client.LastName,
                },

                RedirectUrls = new CallBackUrl
                {
                    Success = _paytrailSettings.RedirectSuccess,
                    Cancel = _paytrailSettings.RedirectCancel
                },

            };

            if (!string.IsNullOrEmpty(_paytrailSettings.CallbackSuccess) && !string.IsNullOrEmpty(_paytrailSettings.CallbackCancel))
            {
                 request.CallbackUrls.Success = _paytrailSettings.CallbackSuccess;
                 request.CallbackUrls.Cancel = _paytrailSettings.CallbackCancel;
            }

            var totalAmount = 0;

            foreach (var item in checkout.Products)
            {

                request.Items.Add(new PaytrailItemModel
                {
                    UnitPrice = item.UnitPrice,
                    Units = item.Units,
                    VatPercentage = item.VatPercentage,
                    ProductCode = item.ProductCode,
                    Description = item.ProductName,
                });

                totalAmount += item.UnitPrice * item.Units;
            }

            request.Amount = totalAmount;

            if (request.Amount != checkout.TotalAmount)
            {
                throw new InvalidOperationException($"Checkout total amount {checkout.TotalAmount} and paytrail request amount {request.Amount} don't match!");
            }

            if (request.Amount <= 0)
            {
                throw new InvalidOperationException($"Paytrail create payment error: request amount can't be zero or negative. Value: {request.Amount}");
            }

            var response = await Paytrail.CreatePaymentAsync(request, _paytrailSettings);

            if (string.IsNullOrEmpty(response.Href))
            {
                throw new InvalidOperationException("Paytrail create payment returned null href link.");
            }

            checkout.TransactionId = response.TransactionId;
            checkout.PaymentStatus = PaymentStatus.Initialized;

            return response.Href;

        }
        catch (Exception ex)
        {
            _logger.LogCritical("Paytrail create payment error! {err}", ex.ToString());
            throw;
        }
    }


    public CallbackResult ValidateCallbackOrWebhook(HttpRequest request)
    {
        try
        {
            //Maybe some fancy automagical mapping later. Fromqueryattribute could be used if inside controller.
            var q = request.Query;
            var CallbackModel = new CallbackModel
            {
                Account = long.Parse(q["checkout-account"]!),
                Algorithm = q["checkout-algorithm"]!,
                Amount = long.Parse(q["checkout-amount"]!),
                SettlementReference = q["checkout-settlement-reference"]!,
                Stamp = q["checkout-stamp"]!,
                Reference = q["checkout-reference"]!,
                TransactionId = q["checkout-transaction-id"]!,
                Status = q["checkout-status"]!,
                Provider = q["checkout-provider"]!,
                Signature = q["signature"]!
            };


            var headers = new Dictionary<string, string>();

            foreach (var header in request.Query)
            {
                headers.Add(header.Key, header.Value!);

            }

            var secret = EnvVariableReader.GetVariable("PAYTRAIL_SECRET");

            var signature = PaytrailCrypto.CalculateHmac(secret, headers, "", headers["checkout-algorithm"]);

            if (!string.Equals(signature, headers["signature"]))
            {
                throw new InvalidOperationException("Callback signature mismatch!");
            }

            //Parse payment providers status string to enum
            var receivedPaymentStatus = (PaytrailPaymentStatuses)Enum.Parse(typeof(PaytrailPaymentStatuses), CallbackModel.Status, true);

            var unifiedPaymentStatus = PaymentStatus.Initialized;

            //Unified statuses are quite same as paytrail statuses because implemented first.
            switch (receivedPaymentStatus)
            {
                case PaytrailPaymentStatuses.New:
                    // The callback status from paytrail should never be "new"
                    throw new InvalidOperationException("Paytrail callback status was New. Something went wrong!");

                case PaytrailPaymentStatuses.Ok:
                    unifiedPaymentStatus = PaymentStatus.Ok;
                    break;

                case PaytrailPaymentStatuses.Fail:
                    unifiedPaymentStatus = PaymentStatus.Fail;
                    break;

                case PaytrailPaymentStatuses.Pending:
                    unifiedPaymentStatus = PaymentStatus.Pending;
                    break;

                case PaytrailPaymentStatuses.Delayed:
                    unifiedPaymentStatus = PaymentStatus.Delayed;
                    break;
            }

            var callbackResult = new CallbackResult
            {
                CheckoutReference = CallbackModel.Reference,
                PaymentStatus = unifiedPaymentStatus,
                PaymentProvider = CallbackModel.Provider,
                TransactionId = CallbackModel.TransactionId
            };

            return callbackResult;

        }
        catch (Exception ex)
        {
            _logger.LogCritical("Paytrail callback validation error: {err}", ex.ToString());
            throw;
        }
    }
}