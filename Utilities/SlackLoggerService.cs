using System.Text;
using System.Text.Json;

namespace YawShop.Utilities;

public class SlackLoggerService : ILogger
{

    private readonly string _webhookUrl;
    private readonly string _categoryName;
    private readonly HttpClient _httpClient;

    public SlackLoggerService(string categoryName)
    {
        _webhookUrl = EnvVariableReader.GetVariable("SLACK");
        _categoryName = categoryName;
        _httpClient = new HttpClient();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Warning;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        // Format the log message
        var message = formatter(state, exception);
        var payload = new
        {
            text = $"[{logLevel}] {_categoryName}: {message}"
        };

        // Send the log message asynchronously
        Task.Run(async () => await SendLogAsync(payload));
    }

    private async Task SendLogAsync(object payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_webhookUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send log to Slack: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log to Slack: {ex.Message}");
        }
    }
}
