namespace YawShop.Utilities.CheckoutCleaner;

public class ConsumeHostedServices : BackgroundService
{
    private readonly ILogger<ConsumeHostedServices> _logger;

    public IServiceProvider Services { get; }

    public ConsumeHostedServices(ILogger<ConsumeHostedServices> logger, IServiceProvider services){
        _logger = logger;
        Services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background checkout cleaner starting.");
        using var scope = Services.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ICleanerTimerService>();
        await scopedProcessingService.ReleaseInitializedCheckouts(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background checkout cleaner stopping.");
        System.Console.WriteLine("stopping");
        await base.StopAsync(stoppingToken);
    }
}