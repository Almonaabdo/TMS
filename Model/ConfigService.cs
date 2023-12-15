using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TMS_Project.Model;
public class ConfigService
{
    #region Fields

    private readonly IConfiguration? _configuration;
    
    #endregion

    #region Cunstructor

    /*
     * METHOD NAME: ConfigService
     * DESCRIPTION: Constructor to initialize the config and set the json path
     *
     * RETURN: void
     */
    public ConfigService()
    {
        try
        {
            // Set the config and json path
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion

    #region Get Invoice

    /*
     * METHOD NAME: GetInvoicePath
     * DESCRIPTION: Method to grab the invoice path from json file
     *
     * RETURN: The path as a string
     */
    public string? GetInvoicePath()
    {
        try
        {
            return _configuration?["Invoice:InvoicePath"];
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogException($"{ex.Message}");
            return null;
        }
    }

    #endregion

    #region Get Connection String

    /*
     * METHOD NAME: GetConnectionString
     * DESCRIPTION: Method to grab the connection string from json file
     *
     * RETURN: The connection string as a string
     */
    public string? GetConnectionString()
    {
        try
        {
            if (_configuration != null) return _configuration.GetConnectionString("RemoteDB");
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogException($"{ex.Message}");
        }

        return null;
    }

    #endregion

    #region Get Backup folder

    /*
     * METHOD NAME: GetBackupPath
     * DESCRIPTION: Method to grab the backup folder path from json file
     *
     * RETURN: The folder path as a string
     */
    public string? GetBackupPath()
    {
        try
        {
            return _configuration?["Backups:BackupFolder"];
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogException($"{ex.Message}");
            return null;
        }
    }

    #endregion

    #region Get log file path

    /*
     * METHOD NAME: GetLogFilePath
     * DESCRIPTION: Method to grab the log files path
     *
     * RETURN: The folder path as a string
     */
    public string? GetLogFilePath()
    {
        try
        {
            return _configuration?["Logging:LogFilePath"];
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogException($"{ex.Message}");
            return null;
        }
    }

    #endregion
 
}