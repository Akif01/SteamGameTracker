using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SteamGameTracker.Logging.Providers;

namespace SteamGameTracker.Logging
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;
        private readonly LogLevel _minimumLogLevel;
        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Task _backgroundTask;
        private static readonly object _logLock = new object(); // Lock object for synchronized access to the log file
        private bool _disposed = false;
        private const int BatchSize = 100;
        private const int ProcessingDelayMs = 100;
        private readonly long _maxFileSizeBytes;
        private readonly bool _enableDailyRotation;

        public FileLogger(string categoryName, string logFilePath, LogLevel minimumLogLevel = LogLevel.Information,
            long maxFileSizeBytes = 10485760, bool enableDailyRotation = true)
        {
            _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            _logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
            _minimumLogLevel = minimumLogLevel;
            _maxFileSizeBytes = maxFileSizeBytes;
            _enableDailyRotation = enableDailyRotation;

            // Ensure the log directory exists
            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Start the background task to process the queue
            _backgroundTask = StartBackgroundTask();
        }

        private Task StartBackgroundTask()
        {
            return Task.Factory.StartNew(ProcessQueueAsync,
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return new LoggerScope();
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{_categoryName}] {message}";

            if (exception != null)
            {
                logMessage += Environment.NewLine + exception.ToString();
            }

            // Enqueue the log message
            _logQueue.Enqueue(logMessage);
        }

        private async Task ProcessQueueAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var logs = new List<string>();

                    // Collect up to BatchSize messages from the queue
                    while (logs.Count < BatchSize && _logQueue.TryDequeue(out var message))
                    {
                        logs.Add(message);
                    }

                    if (logs.Count > 0)
                    {
                        try
                        {
                            WriteLogsToFile(logs);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to write logs to file: {ex.Message}");
                        }
                    }

                    // Wait before checking the queue again to avoid high CPU usage
                    await Task.Delay(ProcessingDelayMs, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested, no need to handle
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error in log processing: {ex}");

                // Try to restart the background task if it fails unexpectedly
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    StartBackgroundTask();
                }
            }
        }

        private void WriteLogsToFile(List<string> logs)
        {
            if (logs == null || logs.Count == 0)
                return;

            string currentFilePath = GetCurrentLogFilePath();

            // Use a lock to ensure that only one thread writes to the file at a time
            lock (_logLock)
            {
                try
                {
                    // Check if we need to rotate based on file size
                    if (_maxFileSizeBytes > 0)
                    {
                        var fileInfo = new FileInfo(currentFilePath);
                        if (fileInfo.Exists && fileInfo.Length >= _maxFileSizeBytes)
                        {
                            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                            string rotatedFilePath = Path.Combine(
                                Path.GetDirectoryName(currentFilePath),
                                $"{Path.GetFileNameWithoutExtension(currentFilePath)}_{timestamp}{Path.GetExtension(currentFilePath)}");

                            File.Move(currentFilePath, rotatedFilePath);
                        }
                    }

                    // Write all logs at once
                    File.AppendAllLines(currentFilePath, logs);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Failed to write logs to file: {ex.Message}");
                }
            }
        }

        private string GetCurrentLogFilePath()
        {
            if (!_enableDailyRotation)
                return _logFilePath;

            string directory = Path.GetDirectoryName(_logFilePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_logFilePath);
            string extension = Path.GetExtension(_logFilePath);

            return Path.Combine(
                directory,
                $"{fileNameWithoutExt}_{DateTime.UtcNow:yyyyMMdd}{extension}");
        }

        public void StopLogging()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();

                try
                {
                    // Wait for the background task to complete with a timeout
                    if (!_backgroundTask.Wait(TimeSpan.FromSeconds(5)))
                    {
                        Console.WriteLine("Warning: Background logging task did not complete within the timeout period.");
                    }
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine($"Error during logger shutdown: {ex.InnerException?.Message}");
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopLogging();
                    _cancellationTokenSource.Dispose();
                }

                _disposed = true;
            }
        }

        ~FileLogger()
        {
            Dispose(false);
        }

        private class LoggerScope : IDisposable
        {
            public void Dispose()
            {
                // No-op for now, but could be extended to support nested logging contexts
            }
        }
    }

    // Extension methods for easier registration
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string logFilePath,
            LogLevel minimumLogLevel = LogLevel.Information,
            long maxFileSizeBytes = 10485760, bool enableDailyRotation = true)
        {
            builder.AddProvider(new FileLoggerProvider(logFilePath, minimumLogLevel, maxFileSizeBytes, enableDailyRotation));
            return builder;
        }
    }
}