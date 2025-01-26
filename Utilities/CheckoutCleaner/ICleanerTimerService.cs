
namespace YawShop.Utilities.CheckoutCleaner;

public interface ICleanerTimerService{

    /// <summary>
    /// When customer initiates a new payment and won't never proceed or cancel it, product quanties, giftcards, discounts and events must be released.
    /// This method should run in background like with some interval (1-10min)?
    /// Method checks if there is checkout items with status 0 -> initialized older than 10 minutes. If so, release product quanties etc.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ReleaseInitializedCheckouts(CancellationToken cancellationToken);
}