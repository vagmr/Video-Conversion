using Serilog;

namespace VideoConverter.Utilities
{
    public static class LoggerConfig
    {
        public static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}