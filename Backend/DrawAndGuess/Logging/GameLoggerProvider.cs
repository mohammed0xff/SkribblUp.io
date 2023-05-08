using System;
using Microsoft.Extensions.Logging;


namespace DrawAndGuess.CustomLogging
{
    public class GameLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new GameLogger(categoryName);
        }

        public void Dispose()
        {
        }

        private class GameLogger : ILogger
        {
            private readonly string _categoryName;

            public GameLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(
                LogLevel logLevel, 
                EventId eventId, 
                TState state, 
                Exception exception, 
                Func<TState, Exception, string> formatter
                )
            {
                var logMessage = formatter(state, exception);
                var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");
                
                // setting color for timestamp
                var coloredTimestamp = $"\u001b[36m{timestamp}\u001b[0m"; 
                
                // setting colors for different log levels.
                var logLevelString = logLevel switch
                {
                    LogLevel.Trace => $"\u001b[35m[trc]\u001b[0m", 
                    LogLevel.Debug => $"\u001b[34m[dbg]\u001b[0m", 
                    LogLevel.Information => $"\u001b[32m[inf]\u001b[0m", 
                    LogLevel.Warning => $"\u001b[33m[wrn]\u001b[0m", 
                    LogLevel.Error => $"\u001b[31m[err]\u001b[0m",
                    LogLevel.Critical => $"\u001b[31m[crt]\u001b[0m",
                    _ => $"\u001b[32m[inf]\u001b[0m"
                };

                Console.Write(coloredTimestamp);
                Console.Write($" {logLevelString} ");
                Console.WriteLine(logMessage);
            }
        }
    }
}
