using System;
using System.IO;
using Devart.Data.MySql;
using Microsoft.Extensions.Configuration;

namespace TMS_Project.Model;

public class AdminModel
{
    /// <summary>
    /// Method to perform database back up operation
    /// </summary>
    public void BackUpDatabase()
    {
        var backUpFolder = Path.Combine(Environment.CurrentDirectory, "Backup");

        // Get current date and time
        var currentDate = DateTime.Now.ToString("yyyy MMMM dd");

        // Construct backup file name with current date
        var fileName = $"backup_{currentDate}.sql";
        // Construct file path
        var filePath = Path.Combine(backUpFolder, fileName);

        try
        {
            // Build configuration to parse connection string from json file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Get config file path
                .AddJsonFile("appsettings.json") // Add json file as the source
                .Build();
                // Grab the connection string
            var connectingString = configuration.GetConnectionString("MyDatabase");

            // Create back up folder directory if it doesnt exist
            Directory.CreateDirectory(backUpFolder);

            // Establish connection to mySql database
            using var connection = new MySqlConnection();
            // Create mySql command
            using var cmd = new MySqlCommand();
            // Use backup library to perform backup operation
            using var backup = new MySqlBackup();
            
            // Assign mySql connection to command
            cmd.Connection = connection;
            // Open db connection
            connection.Open();
            // Export db to file
            backup.ExportToFile(filePath);
            // Close connection
            connection.Close();

            LoggerModel.LogInfo($"Backup operation was completed successfully by Admin. File saved to {filePath}");
        }
        catch (Exception e)
        {
            LoggerModel.LogError($"Error:{e.Message}");
        }


    }

}