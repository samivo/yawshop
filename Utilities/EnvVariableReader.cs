namespace YawShop.Utilities;

public static class EnvVariableReader
{
    // Helper methods for retrieving environment variables
    public static string GetVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key)
               ?? throw new InvalidOperationException($"Environment variable '{key}' is missing.");
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