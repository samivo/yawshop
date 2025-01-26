using Microsoft.EntityFrameworkCore;
using YawShop.Services.StockService;

namespace YawShop.Utilities.CheckoutCleaner;

public class CleanerTimerService : ICleanerTimerService
{
    private readonly ILogger<CleanerTimerService> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IStockService _stock;

    public CleanerTimerService(ILogger<CleanerTimerService> logger, ApplicationDbContext applicationDbContext, IStockService stockService)
    {
        _logger = logger;
        _context = applicationDbContext;
        _stock = stockService;

    }

    public async Task ReleaseInitializedCheckouts(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
           await _context.Database.BeginTransactionAsync();

            try
            {
                //Get the checkouts with payments status "initialized" (0) 
                var checkouts = await _context.Checkouts.Include(checkout => checkout.Products).Where(checkout => checkout.PaymentStatus == Services.CheckoutService.Models.PaymentStatus.Initialized).ToListAsync();

                if (checkouts.Count > 0)
                {
                    _logger.LogInformation("{count} floating checkout's detected.", checkouts.Count);
                    foreach (var checkout in checkouts)
                    {
                        //If checkout is more than 10 minutes old
                        if (checkout.CreatedAt < DateTime.Now.AddMinutes(-10))
                        {
                            await _stock.UpdateQuantitiesAsync(checkout.Reference, false);
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

            

            await Task.Delay(EnvVariableReader.GetVariableAsInt("CLEAN_INTERVAL_MINUTES") * 1000 * 60, cancellationToken);
        }
        
        return;
    }

}