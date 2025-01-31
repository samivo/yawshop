namespace YawShop.Utilities;

public static class EnvVariableReader
{
    // Helper methods for retrieving environment variables
    public static string GetVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (value == null && key.StartsWith("PAYTRAIL_"))
        {
            // Palauta oletusarvoja kehitysympäristössä
            return key switch {
                "PAYTRAIL_REDIRECT_SUCCESS" => "http://localhost:3000/success",
                "PAYTRAIL_REDIRECT_CANCEL" => "http://localhost:3000/cancel",
                _ => throw new InvalidOperationException($"Environment variable '{key}' is missing.")
            };
        }
        return value ?? throw new InvalidOperationException($"Environment variable '{key}' is missing.");
    }

    public static int GetVariableAsInt(string key)
    {
        var value = GetVariable(key);
        if (int.TryParse(value, out var result))
        {
            return result;
        }
        throw new InvalidOperationException($"Environment variable '{key}' must be an integer.");
    }
}
