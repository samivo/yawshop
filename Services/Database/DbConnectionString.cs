
using YawShop.Utilities;

namespace YawShop.Services.Database;

public static class DbConnectionString{

    public static string GetString()
    {
        var Server = EnvVariableReader.GetVariable("DB_SERVER");
        var Database = EnvVariableReader.GetVariable("DB_DATABASE");
        var Port = EnvVariableReader.GetVariableAsInt("DB_PORT");
        var User = EnvVariableReader.GetVariable("DB_USER");
        var Password = EnvVariableReader.GetVariable("DB_PASSWORD");
        var DatetimeKind = "DateTimeKind=Utc"; // Handle database datetime columns as UTC

        return $"Server={Server};Database={Database};User={User};Password={Password};Port={Port};{DatetimeKind}";
    }
}