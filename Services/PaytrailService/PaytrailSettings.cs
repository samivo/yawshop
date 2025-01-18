namespace YawShop.Services.PaytrailService;

/// <summary>
/// Paytrail settings. See paytrail docs further information
/// </summary>
public class PaytrailSettings
{
    /// <summary>
    /// Account number 
    /// </summary>
    public required string Account { get; set; }

    /// <summary>
    /// Account secret 
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    /// Redirect url after successful payment
    /// </summary>
    public required string RedirectSuccess { get; set; }

    /// <summary>
    /// Redirect url after cancel or failed payment
    /// </summary>
    public required string RedirectCancel { get; set; }

    /// <summary>
    /// Callback url after successful payment
    /// </summary>
    public required string CallbackSuccess { get; set; }

    /// <summary>
    /// Callback url after cancel or failed payment
    /// </summary>
    public required string CallbackCancel { get; set; }

}