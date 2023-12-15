using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace TMS_Project.Model;

/// <summary>
///     A singleton class that represents a logging model for TMS, handles logging of information, warnings and errors.
/// </summary>
public class LoggerModel
{
    #region Fields

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Create Nlog instance

    // Create lazy instance for Singleton
    private static readonly Lazy<LoggerModel> LazyInstance = new(() => new LoggerModel());
    private readonly ConfigService _configService;

    #endregion

    #region Constructor

    /*
     * METHOD NAME: LoggerModel
     * DESCRIPTION: Initialize the config
     *
     * RETURN: void
     */
    private LoggerModel()
    {
        _configService = new ConfigService();
        ConfigLog(); // Initialize Nlog config when instance is created
    }

    public static LoggerModel Instance => LazyInstance.Value;

    #endregion


    #region Methods

    /*
     * METHOD NAME: ConfigLog
     * DESCRIPTION: Method to handle Nlog settings, logs all log levels to specified file
     *
     * RETURN: void
     */
    public void ConfigLog()
    {
        var logFilePath = _configService.GetLogFilePath();
        var dynamicFileName = $"{logFilePath}\\Logs_{DateTime.Now:yyyy-MM-dd}.log";
        var config = new LoggingConfiguration();

#pragma warning disable CA2000
        var logToFile = new FileTarget("filelog")
        {
            FileName = dynamicFileName,
            KeepFileOpen = false, // Ensure the log file is closed after each write
            Layout = "${longdate}|${level:uppercase=true}|${message}" // Configure layout without class name
        };
#pragma warning restore CA2000

        config.AddRuleForAllLevels(logToFile);
        // Apply config to NLog
        LogManager.Configuration = config;
    }


    /*
     * METHOD NAME: Log
     * DESCRIPTION: Method to log all levels to file
     *
     * RETURN: void
     */
    private void Log(CustomLogLevel logLevel, string message)
    {
        var logEntry = $"{message}";
        Logger.Log(logLevel.ToNlogLevel(), logEntry);
    }


    /*
     * METHOD NAME: LogInfo
     * DESCRIPTION: Method to log informational message with timestamp
     *
     * RETURN: void
     */
    public void LogInfo(string message)
    {
        Log(CustomLogLevel.Info, message);
    }


    /*
     * METHOD NAME: LogWarning
     * DESCRIPTION: Method to log warning messages with timestamp
     *
     * RETURN: void
     */
    public void LogWarning(string message)
    {
        Log(CustomLogLevel.Warn, message);
    }


    /*
     * METHOD NAME: LogError
     * DESCRIPTION: Method to log error messages
     *
     * RETURN: void
     */
    public void LogError(string message)
    {
        Log(CustomLogLevel.Error, message);
    }


    /*
     * METHOD NAME: LogException
     * DESCRIPTION: Method to log exceptions
     *
     * RETURN: void
     */
    public void LogException(string message)
    {
        Log(CustomLogLevel.Error, $"Exception details: {message}");
    }

    #endregion
}

#region Enums

/*
 * METHOD NAME: CustomLogLevel
 * DESCRIPTION: Enum of the possible log levels
 *
 * RETURN: void
 */
public enum CustomLogLevel
{
    Info,
    Warn,
    Error
}

#endregion

#region Customer Nlog convert class

/// <summary>
///     Class to handle conversion of Custom log levels to Nlog levels
/// </summary>
internal static class LogLevelExt
{
    /*
     * METHOD NAME: ToNlogLevel
     * DESCRIPTION: Convert custom log level to Nlog equivalent log level
     *
     * RETURN: loglevel
     */
    public static LogLevel ToNlogLevel(this CustomLogLevel logLevel)
    {
        return logLevel switch
        {
            CustomLogLevel.Info => LogLevel.Info,
            CustomLogLevel.Error => LogLevel.Error,
            CustomLogLevel.Warn => LogLevel.Warn,
            _ => throw new ArgumentException($@"Unknown log level: {logLevel}", nameof(logLevel))
        };

    }
}
#endregion