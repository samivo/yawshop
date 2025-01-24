using Microsoft.Extensions.Logging;
using YawShop.Utilities;

public class SlackLoggerProvider : ILoggerProvider
{

    public SlackLoggerProvider()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SlackLoggerService(categoryName);
    }

    public void Dispose()
    {
        // Dispose resources if needed
    }
}
