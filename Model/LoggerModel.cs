using NLog;
using System;

namespace TMS_Project.Model
{
    /// <summary>
    /// A singleton class that represents a logging model for TMS, handles logging of information, warnings and errors.
    /// </summary>
    public class LoggerModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Create Nlog instance

        // Create lazy instance for Singleton
        private static readonly Lazy<LoggerModel> LazyInstance = new(() => new LoggerModel());
        private LoggerModel()
        {
            ConfigLog(); // Initialize Nlog config when instance is created
        }

        /// <summary>
        /// Property to access singleton instance
        /// </summary>
        public static LoggerModel Instance => LazyInstance.Value;
        /// <summary>
        /// Method to handle Nlog settings, logs all log levels to specified file
        /// </summary>
        private static void ConfigLog()
        {
            var config = new NLog.Config.LoggingConfiguration(); // Create new Nlog config

            var logToFile = new NLog.Targets.FileTarget("filelog") { FileName = "Logs.txt" }; // Specify file name for logs

            // Add rule so that all log levels are logged to file
            config.AddRuleForAllLevels(logToFile);

            // Apply config to Nlog
            LogManager.Configuration = config;

        }

        /// <summary>
        /// Method to log all levels to file
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        private static void Log(CustomLogLevel logLevel, string message)
        {
            var logEntry = $"{message}";
            Logger.Log(logLevel.ToNlogLevel(), logEntry);

        }

        /// <summary>
        /// Method to log informational message with timestamp
        /// </summary>
        /// <param name="message">The message to be logged</param>
        public static void LogInfo(string message)
        {
            Log(CustomLogLevel.Info, message);
        }

        /// <summary>
        /// Method to log warning messages with timestamp
        /// </summary>
        /// <param name="message">The warning message to be logged as string</param>
        public static void LogWarning(string message)
        {
            Log(CustomLogLevel.Warn, message);
        }

        /// <summary>
        /// Method to log error messages
        /// </summary>
        /// <param name="message">The error message to be logged as string</param>
        public static void LogError(string message)
        {
            Log(CustomLogLevel.Error, message);
        }

        /// <summary>
        /// Method to log exceptions
        /// </summary>
        /// <param name="ex">The exception to be logged</param>
        /// <param name="message"> The additional message to be logged</param>
        public static void LogException(Exception ex, string message)
        {
            Log(CustomLogLevel.Error, $"{message} Exception Details: {ex}");
        }

    }

    /// <summary>
    /// Enum of the possible log levels
    /// </summary>
    public enum CustomLogLevel
    {
        Info,
        Warn,
        Error
    }

    /// <summary>
    /// Class to handle conversion of Custom log levels to Nlog levels
    /// </summary>
    internal static class LogLevelExt
    {
        /// <summary>
        /// Convert custom log level to Nlog equivalent log level
        /// </summary>
        /// <param name="logLevel">The customer log level to convert</param>
        /// <returns>The corresponding Nlog log level</returns>
        /// <exception cref="ArgumentException">Is thrown when an unknown log level is provided</exception>
        public static LogLevel ToNlogLevel(this CustomLogLevel logLevel)
        {
            return logLevel switch
            {
                CustomLogLevel.Info => LogLevel.Info,
                CustomLogLevel.Error => LogLevel.Error,
                CustomLogLevel.Warn => LogLevel.Warn,
                _ => throw new ArgumentException($@"Unknown log level: {logLevel}", nameof(logLevel)),
            };
        }
    }

}