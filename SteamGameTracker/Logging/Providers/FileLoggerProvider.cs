namespace SteamGameTracker.Logging.Providers
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logFilePath;
        private readonly LogLevel _minimumLogLevel;
        private readonly long _maxFileSizeBytes;
        private readonly bool _enableDailyRotation;

        public FileLoggerProvider(string logFilePath, LogLevel minimumLogLevel = LogLevel.Information,
            long maxFileSizeBytes = 10485760, bool enableDailyRotation = true)
        {
            _logFilePath = logFilePath;
            _minimumLogLevel = minimumLogLevel;
            _maxFileSizeBytes = maxFileSizeBytes;
            _enableDailyRotation = enableDailyRotation;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _logFilePath, _minimumLogLevel, _maxFileSizeBytes, _enableDailyRotation);
        }

        public void Dispose()
        {
            // No resources to dispose at the provider level
        }
    }
}