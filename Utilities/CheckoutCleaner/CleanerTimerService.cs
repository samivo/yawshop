using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using YawShop.Services.CheckoutService;
using YawShop.Services.StockService;

namespace YawShop.Utilities.CheckoutCleaner;

public class CleanerTimerService : ICleanerTimerService
{
    private readonly ILogger<CleanerTimerService> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IStockService _stock;

    private readonly ICheckoutService _checkout;

    public CleanerTimerService(ILogger<CleanerTimerService> logger, ApplicationDbContext applicationDbContext, IStockService stockService, ICheckoutService checkoutService)
    {
        _logger = logger;
        _context = applicationDbContext;
        _stock = stockService;
        _checkout = checkoutService;

    }

    public async Task ReleaseInitializedCheckouts(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
           await _context.Database.BeginTransactionAsync();

            try
            {
                //Get the checkouts where payments status "initialized" (0) 
                var checkouts = await _checkout.FindAsync(checkout => checkout.PaymentStatus == Services.CheckoutService.Models.PaymentStatus.Initialized);

                if (checkouts?.Count > 0)
                {
                    _logger.LogInformation("{count} floating checkout's detected.", checkouts.Count);
                    foreach (var checkout in checkouts)
                    {
                        //If checkout is more than 10 minutes old
                        if (checkout.CreatedAt < DateTime.UtcNow.AddMinutes(-10))
                        {
                            await _stock.UpdateQuantitiesAsync(checkout, false);
                            checkout.PaymentStatus = Services.CheckoutService.Models.PaymentStatus.Cancelled;
                            checkout.InternalComment = "Payment has floated more than 10 minutes. Cleaned by bot.";
                            _logger.LogInformation("Cleaned floating checkout. Reference: {ref}.", checkout.Reference);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("No floating checkout's detected.");
                }

                await _context.Database.CommitTransactionAsync();
            }
            catch (System.Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                _logger.LogCritical("Automatic checkout cleaner error: {err}", ex);
                throw;
            }
            finally{
                //Because we are using same context for long period, tracking must be cleared in order to successfully read and update data
                _context.ChangeTracker.Clear();
            }

            

            await Task.Delay(EnvVariableReader.GetVariableAsInt("CLEAN_INTERVAL_MINUTES") * 1000 * 60, cancellationToken);
        }
        
        return;
    }

}